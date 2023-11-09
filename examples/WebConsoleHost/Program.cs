using MATSys;
using MATSys.Commands;

namespace WebConsoleHost
{
    internal class WebConsoleHost
    {
        internal static void Main(params string[] args)
        {
            #region For Webapplicatio usage

            var builder = WebApplication.CreateBuilder(args);

            var startup = new Startup(builder.Configuration);

            startup.ConfigureBuilder(builder);

            startup.ConfigureServices(builder.Services);

            var app = builder.Build();

            startup.Configure(app, builder.Environment);

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
