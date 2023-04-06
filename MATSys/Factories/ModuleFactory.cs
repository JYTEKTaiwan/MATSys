using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Reflection;

namespace MATSys.Factories
{
    /// <summary>
    /// Factory used to create module 
    /// </summary>
    public sealed class ModuleFactory : IModuleFactory
    {

        /// <summary>
        /// section key for recorder in specific section of module
        /// </summary>
        private const string key_recorder = "Recorder";

        /// <summary>
        /// section key for notifier in specific section of module
        /// </summary>
        private const string key_notifier = "Notifier";
        /// <summary>
        /// section key for transceiver in specific section of module
        /// </summary>
        private const string key_transceiver = "Transceiver";

        private readonly ITransceiverFactory _transceiverFactory;
        private readonly INotifierFactory _notifierFactory;
        private readonly IRecorderFactory _recorderFactory;
        private readonly static NLog.ILogger _logger = LogManager.GetCurrentClassLogger();
        public ModuleFactory(IServiceProvider provider)
        {
            _transceiverFactory = provider.GetService<ITransceiverFactory>();
            _notifierFactory = provider.GetService<INotifierFactory>();
            _recorderFactory = provider.GetService<IRecorderFactory>();
        }
        /// <summary>
        /// Create IModule instance using specific section in json file
        /// </summary>
        /// <param name="section">specific section in json file</param>
        /// <returns>IModule instance</returns>
        public IModule CreateModule(IConfigurationSection section)
        {
            try
            {
                _logger.Trace($"Path: {section.Path}");

                string alias = section.GetValue<string>("Alias"); //Get the alias string in json section
                string typeString = section.GetValue<string>("Type"); //Get the type string of Type in json section
                string extAssemblyPath = section.GetValue<string>("AssemblyPath"); //Get the assemblypath string of Type in json section

                _logger.Trace($"Searching for the type named \"{typeString}\"");

                var t = SearchType(typeString, extAssemblyPath);

                if (t == null)
                {
                    throw new InvalidDataException($"Cannot find type {typeString}");
                }
                _logger.Debug($"{t.FullName} is found");

                //Create Transceiver, Notifier, and Recorder
                var trans = _transceiverFactory.CreateTransceiver(section.GetSection(key_transceiver));
                var noti = _notifierFactory.CreateNotifier(section.GetSection(key_notifier));
                var rec = _recorderFactory.CreateRecorder(section.GetSection(key_recorder));
                _logger.Debug($"[{alias}]{typeString} is created with {trans.Alias},{noti.Alias},{rec.Alias}");

                //Create instance and return 
                var obj = (IModule)Activator.CreateInstance(t);

                obj.Alias = alias;
                obj.Configure(section);
                obj.InjectRecorder(rec);
                obj.InjectNotifier(noti);
                obj.InjectTransceiver(trans);
                return obj;
            }
            catch (Exception ex)
            {

                throw ex;
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
        public static IModule CreateNew(string assemblyPath, string typeName, object args)
        {
            var t = SearchType(typeName, assemblyPath);
            //Create instance and return 
            var obj = (IModule)Activator.CreateInstance(t);
            obj.Configure(args);
            return obj;
        }
    }
}