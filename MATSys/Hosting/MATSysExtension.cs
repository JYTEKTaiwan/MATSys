using MATSys.Factories;
using MATSys.Hosting.Scripting;
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
            return hostBuilder.ConfigureServices(service =>
            service.AddHostedService<ModuleHubBackgroundService>()
                    .AddSingleton<IModuleFactory, ModuleFactory>()
                    .AddSingleton<IRecorderFactory, RecorderFactory>()
                    .AddSingleton<INotifierFactory, NotifierFactory>()
                    .AddSingleton<IRunnerFactory, RunnerFactory>()
                    .AddSingleton<ITransceiverFactory, TransceiverFactory>()
                    .AddSingleton<TestScriptContext>()
                    .AddSingleton<AnalyzerLoader>()
            )
            .ConfigureLogging(logging =>
                logging.AddNLog()
            )
            .ConfigureAppConfiguration(ctxt =>
            {
                ctxt.Sources.Clear();
                ctxt.SetBasePath(Directory.GetCurrentDirectory());
                ctxt.AddJsonFile("appsettings.json");
            }

            );

        }

        public static IRunner GetRunner(this IServiceProvider provider)
        {
            return provider.GetServices<IHostedService>().OfType<ModuleHubBackgroundService>().FirstOrDefault().GetRunner();
        }
    }
}
