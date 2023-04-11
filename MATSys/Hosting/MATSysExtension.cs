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

        /// <summary>
        /// Replace the IRunner service when configurating the Host
        /// </summary>
        /// <typeparam name="T">Type of the IRnner </typeparam>
        /// <param name="hostBuilder">host builder instance</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder ReplaceRunnerService<T>(this IHostBuilder hostBuilder) where T : IRunner
        {
            return hostBuilder.ConfigureServices(x =>
            x.RemoveAll<IRunner>()
            .AddSingleton(typeof(IRunner), typeof(T))
            );
        }

        /// <summary>
        /// Get the IRunner insatnce
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IRunner GetMATSysRunner(this IHost host)
        {
            return host.Services.GetRequiredService<IRunner>();
        }

        /// <summary>
        /// Get the Root of the configuration in the host
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IConfiguration GetConfigurationRoot(this IServiceProvider provider)
        {
            return provider.GetRequiredService<IConfiguration>();
        }
        /// <summary>
        /// Get the specified section of the configuration in the host
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="sectionKey"></param>
        /// <returns></returns>
        public static IConfigurationSection GetConfigurationSection(this IServiceProvider provider, string sectionKey)
        {
            return provider.GetRequiredService<IConfiguration>().GetSection(sectionKey);
        }
        /// <summary>
        /// Get the collection of information for Module in the host
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Dictionary<string, IConfigurationSection> GetAllModuleInfos(this IServiceProvider provider)
        {
            return provider.GetConfigurationSection("MATSys:Modules").GetChildren().ToDictionary(x => x["Alias"]);
        }


    }
}
