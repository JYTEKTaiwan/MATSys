#define use_UDS
using MATSys;
using MATSys.Commands;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MATSys.Hosting;
using ProtoBuf.Grpc.Server;

namespace WebConsoleHost
{
    internal class WebConsoleHost
    {
        internal static void Main(params string[] args)
        {
            #region For Webapplicatio usage

            var builder = WebApplication.CreateBuilder(args);

            //setup configuration
            builder.Configuration.AddConfigurationInMATSys();

            //setup logging
            builder.Logging.AddNlogInMATSys();

            //setup gRPC socket

#if use_UDS
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
#endif

            //setup services
#if use_UDS
            builder.Services.AddCodeFirstGrpc();

#else
            builder.Services.AddGrpc();
#endif

            builder.Services.AddSingleton<GreeterService>();
            builder.Services.AddMATSysService();

            //build
            var app = builder.Build();

            //mapping grpcServices
            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            //run
            app.Run();

            #endregion


        }
    }

    internal class TestDevice : ModuleBase
    {

        [MATSysCommand]
        public string Hello(string input)
        {
            return $"Rogered by Dev1! [{input}]";

        }
    }

}
