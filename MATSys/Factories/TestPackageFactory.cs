using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Reflection;

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
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();
        public TestPackageFactory(IServiceProvider provider)
        {
            _serviceProvider = provider;
            _notifierFactory = _serviceProvider.GetRequiredService<INotifierFactory>();

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

                var t = SearchType(setting.Type, setting.AssemblyPath);

                if (t == null)
                {
                    throw new InvalidDataException($"Cannot find type {setting.Type}");
                }
                _logger.Debug($"{t.FullName} is found");

                //create notifier
                var _noti = _notifierFactory.CreateNotifier(section.GetSection(key_notifier));


                //Create instance and return 
                var obj = (ITestPackage)Activator.CreateInstance(t);
                obj.InjectServiceProvider(_serviceProvider);
                obj.InjectNotifier(_noti);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IEnumerable<ITestPackage> LoadTestPackagesFromSetting()
        {
            var config = _serviceProvider.GetRequiredService<IConfiguration>();
            foreach (var item in config.GetSection("MATSys:TestPackages").GetChildren())
            {
                yield return CreateTestPackage(item);
            }

        }

        private static Type SearchType(string type, string extAssemPath)
        {
            if (!string.IsNullOrEmpty(type)) // return EmptyRecorder if type is empty or null
            {
                // 1.  Look up the existed assemlies in GAC
                // 1.y if existed, get the type directly and overrider the variable t
                // 1.n if not, dynamically load the assembly from the section "AssemblyPath" and search for the type

                var typeName = Assembly.CreateQualifiedName(type, type).Split(',')[0];
                _logger.Trace($"Searching the entry assemblies");
                Type dummy;
                if (Assembly.GetEntryAssembly().GetTypes().FirstOrDefault(x => x.FullName == typeName) != null)
                {
                    dummy = Assembly.GetEntryAssembly().GetTypes().FirstOrDefault(x => x.FullName == typeName);
                    _logger.Debug($"Found \"{dummy.Name}\"");
                    return dummy;
                }
                else if (Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.FullName == typeName) != null)
                {
                    dummy = Assembly.GetEntryAssembly().GetTypes().FirstOrDefault(x => x.FullName == typeName);
                    _logger.Debug($"Found \"{dummy.Name}\"");
                    return dummy;
                }
                else if (Assembly.GetCallingAssembly().GetTypes().FirstOrDefault(x => x.FullName == typeName) != null)
                {
                    dummy = Assembly.GetEntryAssembly().GetTypes().FirstOrDefault(x => x.FullName == typeName);
                    _logger.Debug($"Found \"{dummy.Name}\"");
                    return dummy;

                }
                else
                {

                    _logger.Trace($"Searching the external path \"{extAssemPath}\"");

                    //load the assembly from external path
                    var assem = DependencyLoader.LoadPluginAssemblies(new string[] { extAssemPath }).First();

                    dummy = assem.GetType(type);
                    if (dummy != null)
                    {
                        _logger.Debug($"Found \"{dummy.Name}\"");
                        return dummy;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }

        }

    }
}