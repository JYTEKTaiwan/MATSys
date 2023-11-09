using MATSys.Factories;


namespace MATSys.Hosting
{
    /// <summary>
    /// Extension class for builder and app 
    /// </summary>
    public static class MATSysExtension
    {

        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
               //.AddEnvironmentVariables()
               .AddJsonFile("appsettings.json", optional: true)
               .Build();

        /// <summary>
        /// Inject the neccesary services into Host services
        /// </summary>
        /// <param name="services">service collection from host builder</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddMATSysService(this IServiceCollection services)
        {
            services.AddSingleton<IRecorderFactory, RecorderFactory>()
                       .AddSingleton<INotifierFactory, NotifierFactory>()
                       .AddSingleton<ITransceiverFactory, TransceiverFactory>()
                        .AddSingleton<IModuleFactory, ModuleFactory>()
                        .AddSingleton<ModuleActivator>();
            // foreach (var section in Configuration.GetSection("MATSys:Modules").GetChildren())
            // {
            //     string typeString = section.GetValue<string>("Type"); //Get the type string of Type in json section
            //     string extAssemblyPath = section.GetValue<string>("AssemblyPath"); //Get the assemblypath string of Type in json section
            //     var t = TypeParser.SearchType(typeString, extAssemblyPath);
            //     services.AddSingleton(typeof(IModule), t);
            // }
            return services;
        }
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
