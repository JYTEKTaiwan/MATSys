using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;


namespace MATSys.Hosting
{
    /// <summary>
    /// Extension class for builder and app 
    /// </summary>
    public static class MATSysExtension
    {

        /// <summary>
        /// Inject the neccesary services into Host services
        /// </summary>
        /// <param name="services">service collection from host builder</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddMATSysService(this IServiceCollection services)
        => services.AddSingleton<IRecorderFactory, RecorderFactory>()
                   .AddSingleton<INotifierFactory, NotifierFactory>()
                   .AddSingleton<ITransceiverFactory, TransceiverFactory>()
                   .AddSingleton<IModuleFactory, ModuleFactory>()
                   .AddHostedService<ModuleActivatorService>();

        /// <summary>
        /// Inject NLog service into logger builder in host
        /// </summary>
        /// <param name="loggingBuilder">log builder service</param>
        /// <returns>ILoggingBuilder</returns>
        public static ILoggingBuilder AddNlogInMATSys(this ILoggingBuilder loggingBuilder) => loggingBuilder.AddNLog();

        /// <summary>
        /// Inject configuration serrvice into configurationbuilder 
        /// </summary>
        /// <param name="configBuilder">configuration builder </param>
        /// <returns>IConfigurationBuilder</returns>
        public static IConfigurationBuilder AddConfigurationInMATSys(this IConfigurationBuilder configBuilder)
        {
            var config = new ConfigurationBuilder()
            //.AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
            configBuilder.Sources.Clear();
            configBuilder.AddConfiguration(config);
            return configBuilder;
        }
        /// <summary>
        /// Get the Root of the configuration in the host
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IConfiguration GetConfigurationRoot(this IServiceProvider provider)
        => provider.GetRequiredService<IConfiguration>();

        /// <summary>
        /// Get the specified section of the configuration in the host
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="sectionKey"></param>
        /// <returns></returns>
        public static IConfigurationSection GetConfigurationSection(this IServiceProvider provider, string sectionKey)
        => provider.GetRequiredService<IConfiguration>().GetSection(sectionKey);




        /// <summary>
        /// Get the module from the host using alias name 
        /// </summary>
        /// <param name="provider">service provider</param>
        /// <param name="alias">alias name</param>        
        /// <returns>IModule implementation</returns>
        public static IModule GetModule(this IServiceProvider provider, string alias) => provider.GetServices<IHostedService>().OfType<ModuleActivatorService>().Single().GetModule(alias);

        /// <summary>
        /// List all the active modules in the memory
        /// </summary>                
        /// <returns>Collection of <see cref="IModule"/> </returns>
        public static IModule[] GetModules(this IServiceProvider provider) => provider.GetServices<IHostedService>().OfType<ModuleActivatorService>().Single().ActiveModules;



    }
}
