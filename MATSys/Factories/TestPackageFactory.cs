using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace MATSys.Factories
{
    /// <summary>
    /// Factory used to create module 
    /// </summary>
    public sealed class TestPackageFactory
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly INotifierFactory _notifierFactory;
        private const string key_notifier = "Notifier";
        private readonly IRecorderFactory _recorderFactory;
        private const string key_recorder = "Recorder";

        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Ctor 
        /// </summary>
        /// <param name="provider">service collection from host</param>
        public TestPackageFactory(IServiceProvider provider)
        {
            _serviceProvider = provider;
            _notifierFactory = _serviceProvider.GetRequiredService<INotifierFactory>();
            _recorderFactory = _serviceProvider.GetRequiredService<IRecorderFactory>();

        }
        /// <summary>
        /// Create IModule instance using specific section in json file
        /// </summary>
        /// <param name="section">specific section in json file</param>
        /// <returns>IModule instance</returns>
        public ITestPackage CreateTestPackage(IConfigurationSection section)
        {
            try
            {
                _logger.Trace($"Path: {section.Path}");
                var setting = section.Get<TestPackageContext>();

                _logger.Trace($"Searching for the type named \"{setting.Type}\"");

                var t = TypeParser.SearchType(setting.Type, setting.AssemblyPath);

                if (t == null)
                {
                    throw new InvalidDataException($"Cannot find type {setting.Type}");
                }
                _logger.Debug($"{t.FullName} is found");

                //create notifier
                var _noti = _notifierFactory.CreateNotifier(section.GetSection(key_notifier));
                var _rec = _recorderFactory.CreateRecorder(section.GetSection(key_recorder));

                //Create instance and return
                var instance = Activator.CreateInstance(t);
                if (instance != null)
                {
                    var obj = (ITestPackage)instance;
                    obj.InjectServiceProvider(_serviceProvider);
                    obj.InjectNotifier(_noti);
                    obj.InjectRecorder(_rec);
                    return obj;
                }
                else
                {
                    throw new NullReferenceException($"Cannot create instance from type '{t.Name}'");
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}