
using MATSys.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProtoBuf.Grpc.Server;
using WebConsoleHost;

public class Startup
{
    public IConfiguration configRoot
    {
        get;
    }
    public Startup(IConfiguration configuration)
    {
        configRoot = configuration;
    }

    public void ConfigureBuilder(WebApplicationBuilder builder)
    {
        File.Delete(Path.Combine(Path.GetTempPath(), "socket.tmp"));
        var socketPath = Path.Combine(Path.GetTempPath(), "socket.tmp");
        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenUnixSocket(socketPath, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });
        builder.Configuration.AddConfigurationInMATSys();
        builder.Logging.AddNlogInMATSys();

    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCodeFirstGrpc();
        services.AddSingleton<GreeterService>();
        services.AddMATSysService();
    }
    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        app.MapGrpcService<GreeterService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

    }
}