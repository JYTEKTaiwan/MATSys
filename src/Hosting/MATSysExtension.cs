using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace MATSys.Hosting
{
    /// <summary>
    /// Extension class for IHostBuilder
    /// </summary>
    public static class MATSysExtension
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
               //.AddEnvironmentVariables()
               .AddJsonFile("appsettings.json", optional: true)
               .Build();

        /*
        /// <summary>
        /// Insert MATSys and its related libraies into IHostBuilder
        /// </summary>
        /// <param name="hostBuilder">instance of IHostBuilder</param>
        /// <returns></returns>
        public static IHostBuilder UseMATSys(this IHostBuilder hostBuilder)
        {

            return hostBuilder.ConfigureServices(service =>
                service.AddMATSysService())
            .ConfigureLogging(logging =>
                logging.AddNLog())
            .ConfigureAppConfiguration(ctxt =>
            {
                ctxt.Sources.Clear();
                ctxt.AddConfiguration(Configuration);
            });

        }
        */
        public static IServiceCollection AddMATSysService(this IServiceCollection services)
        {
            services.AddSingleton<IRecorderFactory, RecorderFactory>()
                       .AddSingleton<INotifierFactory, NotifierFactory>()
                       .AddSingleton<ITransceiverFactory, TransceiverFactory>();
            foreach (var section in Configuration.GetSection("MATSys:Modules").GetChildren())
            {
                string typeString = section.GetValue<string>("Type"); //Get the type string of Type in json section
                string extAssemblyPath = section.GetValue<string>("AssemblyPath"); //Get the assemblypath string of Type in json section
                var t = TypeParser.SearchType(typeString, extAssemblyPath);
                services.AddSingleton(typeof(IModule), t);
            }
            return services;
        }
        public static ILoggingBuilder AddNlogInMATSys(this ILoggingBuilder loggingBuilder) => loggingBuilder.AddNLog();
        public static IConfigurationBuilder AddConfigurationInMATSys(this IConfigurationBuilder configBuilder)
        {
            configBuilder.Sources.Clear();
            configBuilder.AddConfiguration(Configuration);
            return configBuilder;
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
