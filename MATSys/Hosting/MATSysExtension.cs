using MATSys.Factories;
using MATSys.Hosting.Scripting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            service.AddSingleton<IModuleFactory, ModuleFactory>()
                    .AddSingleton<IRecorderFactory, RecorderFactory>()
                    .AddSingleton<INotifierFactory, NotifierFactory>()
                    .AddSingleton<ITransceiverFactory, TransceiverFactory>()
                    .AddSingleton<TestPackageFactory>()
                    .AddSingleton<IRunner, ScriptRunner>()
            )
            .ConfigureLogging(logging =>
                logging.AddNLog()
            )
            .ConfigureAppConfiguration(ctxt =>
            {
                ctxt.Sources.Clear();
                ctxt.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                ctxt.AddJsonFile("appsettings.json");
            }

            );

        }

        public static IHostBuilder ReplaceRunnerService<T>(this IHostBuilder hostBuilder) where T : IRunner
        {
            return hostBuilder.ConfigureServices(x =>
            x.RemoveAll<IRunner>()
            .AddSingleton(typeof(IRunner), typeof(T))
            );
        }

        public static IRunner GetMATSysRunner(this IHost host)
        {
            return host.Services.GetService<IRunner>();
        }

        public static IConfiguration GetConfigurationRoot(this IServiceProvider provider)
        {
            return provider.GetRequiredService<IConfiguration>();
        }
        public static IConfigurationSection GetConfigurationSection(this IServiceProvider provider, string sectionKey)
        {
            return provider.GetRequiredService<IConfiguration>().GetSection(sectionKey);
        }
        public static Dictionary<string, IConfigurationSection> GetAllModuleInfos(this IServiceProvider provider)
        {
            return provider.GetConfigurationSection("MATSys:Modules").GetChildren().ToDictionary(x => x["Alias"]);
        }
        public static IConfigurationSection GetRunnerInfo(this IServiceProvider provider)
        {
            return provider.GetConfigurationSection("MATSys:Runner");
        }

    }
}
