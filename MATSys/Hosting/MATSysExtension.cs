using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace MATSys.Hosting
{
    /// <summary>
    /// Extension class for IHostBuilder
    /// </summary>
    public static class MATSysExtension
    {
        /// <summary>
        /// Insert MATSys and its related libraies into IHostBuilder
        /// </summary>
        /// <param name="hostBuilder">instance of IHostBuilder</param>
        /// <returns></returns>
        public static IHostBuilder UseMATSys(this IHostBuilder hostBuilder)
        {

            var config = new ConfigurationBuilder()
   .SetBasePath(Directory.GetCurrentDirectory())
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .Build();
            //if (config.GetSection("MATSys:EnableNLogInJsonFile").Get<bool>())
            //{
            //    NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
            //}

            return hostBuilder.ConfigureServices(service =>
            service.AddHostedService<ModuleHubBackgroundService>()
                    .AddSingleton<IModuleFactory, ModuleFactory>()
                    .AddSingleton<IRecorderFactory, RecorderFactory>()
                    .AddSingleton<INotifierFactory, NotifierFactory>()
                    .AddSingleton<ITransceiverFactory, TransceiverFactory>()
                    .AddSingleton<AutoTestScheduler>()
            )
            .ConfigureLogging(logging =>
                logging.AddNLog()
            );

        }

        /// <summary>
        /// Extended method to get the handle of MATSys from IServiceProvider instance
        /// </summary>
        /// <param name="provider">instance of IServiceProvider</param>
        /// <returns>instance of ModuleHubBackgroundService</returns>
        public static ModuleHubBackgroundService GetMATSysHandle(this IServiceProvider provider)
        {
            return provider.GetServices<IHostedService>().OfType<ModuleHubBackgroundService>().FirstOrDefault()!;
        }
    }
}
