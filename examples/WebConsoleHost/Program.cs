using MATSys;
using MATSys.Commands;
using MATSys.Hosting;
using MATSys.Hosting.Grpc;

namespace WebConsoleHost
{
    internal class WebConsoleHost
    {
        internal static void Main(params string[] args)
        {
            #region For Webapplicatio usage

            var builder = WebApplication.CreateBuilder(args);


            builder.Configuration.UseConfigurationInMATSys();
            builder.Logging.AddNlogInMATSys();
            builder.Host.ConfigureServices((c,s)=>s.AddMATSysService(c));
            builder.AddGrpcService();

            //build
            var app = builder.Build();
            app.MapGrpcServices();
            //app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            //run
            app.Run();

            #endregion


        }
    }

    internal class TestDevice : ModuleBase
    {
        public override object Configuration { get; set; }
        [MATSysCommand]
        public string Hello(string input)
        {
            return $"Rogered by Dev1! [{input}]";

        }
    }

}
