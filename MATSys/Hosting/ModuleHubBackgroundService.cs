using MATSys.Commands;
using MATSys.Factories;
using MATSys.Hosting.Scripting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;

namespace MATSys.Hosting
{
    /// <summary>
    /// A service class that handles all modules in the background, including the ability to run in script mode.
    /// </summary>
    public sealed class ModuleHubBackgroundService : BackgroundService
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly IRunner _runner;
        private readonly bool _scriptMode = false;

        /// <summary>
        /// Collection for all modules created the background servie
        /// </summary>
        public Dictionary<string, IModule> Modules { get; } = new Dictionary<string, IModule>();

        /// <summary>
        /// Constructor for the background service
        /// </summary>
        /// <param name="services">IServiceProvider instance</param>
        /// <exception> throw <see cref="Exception"/> for any exception</exception>
        public ModuleHubBackgroundService(IServiceProvider services)
        {
            try
            {
                _logger = LogManager.GetCurrentClassLogger();
                _config = services.GetRequiredService<IConfiguration>().GetSection("MATSys");
                _scriptMode = _config.GetValue<bool>("ScriptMode");
                _moduleFactory = services.GetRequiredService<IModuleFactory>();
                foreach (var item in _config.GetSection("Modules").GetChildren())
                {
                    Modules.Add(item.GetValue<string>("Alias"), _moduleFactory.CreateModule(item));
                }
                foreach (var item in Modules)
                {
                    //Assign the LocalPeers properties (Modules can access each other instance locally)
                    item.Value.LocalPeers = Modules;
                    item.Value.Provider = services;
                }
                if (_scriptMode)
                {
                    var script = services.GetRequiredService<AutomationTestScriptContext>();
                    _runner = (ScriptRunner)Activator.CreateInstance(typeof(ScriptRunner),script, Modules);
                }
                else
                {
                    _runner = (ManualRunner)Activator.CreateInstance(typeof(ManualRunner),  Modules);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ModuleHub Initialzation failed", ex);
            }
        }

        /// <summary>
        /// This method will be called immediately once the host is started
        /// </summary>
        /// <param name="stoppingToken">Stop token</param>
        /// <returns>Task of the execution logic</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                //temperarily doing nothing
            });
        }

        public IRunner GetRunner()
        {
            return _runner;
        }
    }

}