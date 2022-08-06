using MATSys.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace MATSys.Hosting
{
    public static class MATSysExtension
    {
        public static IHostBuilder UseMATSys(this IHostBuilder hostBuilder)
        {

            var config = new ConfigurationBuilder()
   .SetBasePath(System.IO.Directory.GetCurrentDirectory())
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .Build();
            if (config.GetSection("MATSys:EnableNLogInJsonFile").Get<bool>())
            {
                NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
            }

            return hostBuilder.ConfigureServices(service =>
            service.AddHostedService<ModuleHubBackgroundService>()
                    .AddSingleton<IModuleFactory, ModuleFactory>()
                    .AddSingleton<IRecorderFactory, RecorderFactory>()
                    .AddSingleton<INotifierFactory, NotifierFactory>()
                    .AddSingleton<ITransceiverFactory, TransceiverFactory>()
            )
            .ConfigureLogging(logging =>
                logging.AddNLog()
            );

        }

        public static ModuleHubBackgroundService GetMATSysHandle(this IServiceProvider provider)
        {
            return provider.GetServices<IHostedService>().OfType<ModuleHubBackgroundService>().FirstOrDefault();
        }
    }
}
