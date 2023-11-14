using System.Net;
using MATSys.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Server;

namespace MATSys.Hosting.Grpc;

/// <summary>
/// Extension class for grpc service
/// </summary>
public static class GrpcExtension
{

  /// <summary>
  /// Add grpc service into builder
  /// </summary>
  /// <returns><see cref="WebApplicationBuilder"/> </returns>
  public static WebApplicationBuilder AddGrpcService(this WebApplicationBuilder builder)
  {
    //get the configuration from json file
    var config = builder.Configuration.GetSection(GrpcConfiguration.sectionKey).Get<GrpcConfiguration>();
    if (config == null) throw new ArgumentNullException("Cannot find the configuration section");
    else
    {

      builder.WebHost.ConfigureKestrel(serverOptions =>
      {
        if (config.EnableUDS)
        {
          var path = string.IsNullOrEmpty(config.UDSFileName) ? "socket.tmp" : config.UDSFileName;
          var socketPath = Path.Combine(Path.GetTempPath(), path);
          File.Delete(socketPath);
          serverOptions.ListenUnixSocket(socketPath, listenOptions =>
          {
            listenOptions.Protocols = HttpProtocols.Http2;

          });
        }
        else
        {
          var port = config.Port > 0 ? config.Port : 5000;
          IPAddress? ip=IPAddress.None;
          if (IPAddress.TryParse(config.IPAddress, out ip))
          {
            serverOptions.Listen(ip, port, listenOptions =>
            {
              listenOptions.Protocols = HttpProtocols.Http2;
            });
          }
          else
          {
            serverOptions.ListenLocalhost(port, listenOptions =>
            {
              listenOptions.Protocols = HttpProtocols.Http2;
              listenOptions.UseHttps();
            });
          }


        }

      });
    }

    //Mark the type as singleton or not
    foreach (var item in config.Services)
    {
      var t = TypeParser.SearchType(item.Type, item.AssemblyPath);
      if (t != null && item.Singleton)
      {
        builder.Services.AddSingleton(t);
      }
    }

    //add code-first grpc service
    builder.Services.AddCodeFirstGrpc();
    return builder;
  }


  /// <summary>
  /// Map the grpc service to application
  /// </summary>
  /// <returns><see cref="WebApplication"/> </returns>
  public static WebApplication MapGrpcServices(this WebApplication app)
  {
    var config = app.Configuration.GetSection(GrpcConfiguration.sectionKey).Get<GrpcConfiguration>();
    if (config == null) throw new ArgumentNullException("Cannot find the configuration section");
    else
    {
      foreach (var item in config.Services)
      {
        var t = TypeParser.SearchType(item.Type, item.AssemblyPath);
        if (t != null)
        {
          var mi = MethodInvoker.Create(typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService")!.MakeGenericMethod(t));
          mi.Invoke(app);
        }
      }
      return app;
    }


  }

}

/// <summary>
/// POCO for configuration data
/// </summary>
internal class GrpcConfiguration
{
  /* json format
  "MATSys:Grpc":{
        "EnableUDS":true,
        "UDSFileName":"",
        "IPAddress":"",
        "Port":5000,
        "Services":[
          {
            "AssemblyPath":"",
            "Type":",
            "Singleton":true
          }
        ]
      }
  */
  /// <summary>
  /// Section key for json configuration
  /// </summary>
  public const string sectionKey = "MATSys:Grpc";

  /// <summary>
  /// Enable the Unix Domain Socket (for Inter-process usage)
  /// </summary>
  /// <value>true</value>
  public bool EnableUDS { get; set; } = true;
  /// <summary>
  /// UDS file name
  /// </summary>
  /// <value>"socket.tmp"</value>
  public string UDSFileName { get; set; } = "socket.tmp";

  /// <summary>
  /// IP Address (ignore if <see cref="EnableUDS"/> is true)
  /// </summary>
  /// <value></value>
  public string IPAddress { get; set; } = "";
  /// <summary>
  /// Port number
  /// </summary>
  /// <value>5000</value>
  public int Port { get; set; } = 5000;
  /// <summary>
  /// Services information
  /// </summary>
  public IEnumerable<GrpcServiceContext> Services { get; set; } = new List<GrpcServiceContext>();


  /// <summary>
  /// Grpc service context
  /// </summary>
  internal class GrpcServiceContext
  {

    /// <summary>
    /// external assembly path(empty if the type is in GAC)
    /// </summary>
    /// <value>""</value>
    public string AssemblyPath { get; set; } = "";
    /// <summary>
    /// Type string to search (fullname)
    /// </summary>
    /// <value>""</value>
    public string Type { get; set; } = "";
    /// <summary>
    /// Indicate the service is singleton or not
    /// </summary>
    /// <value>false</value>
    public bool Singleton { get; set; } = false;
  }
}

