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
    public sealed class MATSysBackgroundService : BackgroundService
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly IRunnerFactory _runnerFactory;
        private readonly TestPackageFactory _packageFactory;
        private readonly IConfigurationSection _config;
        private readonly ILogger _logger;
        private readonly IRunner _runner;


        /// <summary>
        /// Constructor for the background service
        /// </summary>
        /// <param name="services">IServiceProvider instance</param>
        /// <exception> throw <see cref="KeyNotFoundException"/>throw if MATSys section is not existed in json file</exception>
        /// <exception> throw <see cref="InvalidDataException"/>throw if type of module is not found</exception>
        /// <exception> throw <see cref="Exception"/>for all unhandled exception</exception>
        public MATSysBackgroundService(IServiceProvider services)
        {
            try
            {
                _logger = LogManager.GetCurrentClassLogger();
                _config = GetMATSysSection(services);
                _runner = services.GetRequiredService<IRunner>();

            }
            catch (KeyNotFoundException ex)
            {
                throw ex;
            }
            catch (InvalidDataException ex)
            {
                throw ex;
            }
            catch (DirectoryNotFoundException ex)
            {
                throw ex;
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception($"ModuleHub Initialzation failed", ex);
            }
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
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

        private IConfigurationSection GetMATSysSection(IServiceProvider services)
        {
            var config = services.GetRequiredService<IConfiguration>();
            if (!config.GetSection("MATSys").Exists())
            {
                throw new KeyNotFoundException("MATSys section is not found in appsettings.json");
            }
            else
            {
                return config.GetSection("MATSys");
            }
        }




    }

}