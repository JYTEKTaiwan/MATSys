using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Diagnostics;
using System.Reflection;
using static System.Collections.Specialized.BitVector32;

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

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="provider">service provide from host</param>
        public ModuleFactory(IServiceProvider provider)
        {
            _transceiverFactory = provider.GetRequiredService<ITransceiverFactory>();
            _notifierFactory = provider.GetRequiredService<INotifierFactory>();
            _recorderFactory = provider.GetRequiredService<IRecorderFactory>();
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
                var instance = Activator.CreateInstance(t);
                if (instance != null)
                {
                    var obj = (IModule)instance;
                    obj.Alias = alias;
                    obj.Configure(section);
                    obj.InjectRecorder(rec);
                    obj.InjectNotifier(noti);
                    obj.InjectTransceiver(trans);
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

        /// <summary>
        /// Create new instance from external dll path
        /// </summary>
        /// <param name="assemblyPath">external dll path</param>
        /// <param name="typeName">name of type</param>
        /// <param name="args">configuration instance</param>
        /// <returns>IModule instance</returns>
        public static IModule CreateNew(string assemblyPath, string typeName, object args)
        {
            var t = SearchType(typeName, assemblyPath);
            //Create instance and return
            var instance = Activator.CreateInstance(t);
            if (instance != null)
            {
                var obj = (IModule)instance;                
                obj.Configure(args);             
                return obj;
            }
            else
            {
                throw new NullReferenceException($"Cannot create instance from type '{t.Name}'");
            }
        }

        /// <summary>
        /// Create new instance from known type
        /// </summary>
        /// <typeparam name="T">Type of the IModule implementation</typeparam>
        /// <param name="args">configuration instance</param>
        /// <returns>IModule instance</returns>
        public static IModule CreateNew<T>(object args) where T : IModule
        {
            var t = typeof(T);
            //Create instance and return
            var instance = Activator.CreateInstance(t);
            if (instance != null)
            {
                var obj = (IModule)instance;
                obj.Configure(args);
                return obj;
            }
            else
            {
                throw new NullReferenceException($"Cannot create instance from type '{t.Name}'");
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
                if (SearchTypeInEntryAssemblies(typeName) is Type t_entry && t_entry != null)
                {
                    _logger.Debug($"Found \"{t_entry.Name}\" in entry Assemblies");
                    return t_entry;
                }
                else if (SearchTypeInExecutingAssemblies(typeName) is Type t_exec && t_exec != null)
                {
                    _logger.Debug($"Found \"{t_exec.Name}\" in executing Assemblies");
                    return t_exec;
                }
                else if (SearchTypeInCallingAssemblies(typeName) is Type t_calling && t_calling != null)
                {
                    _logger.Debug($"Found \"{t_calling.Name}\" in calling Assemblies");
                    return t_calling;

                }
                else
                {

                    _logger.Trace($"Searching the external path \"{extAssemPath}\"");

                    //load the assembly from external path
                    var assem = DependencyLoader.LoadPluginAssemblies(new string[] { extAssemPath }).First();

                    var t_ext = assem.GetType(type);
                    if (t_ext != null)
                    {
                        _logger.Debug($"Found \"{t_ext.Name}\"");
                        return t_ext;
                    }
                    else
                    {
                        return null!;
                    }
                }
            }
            else
            {
                return null!;
            }

        }
        private static Type? SearchTypeInEntryAssemblies(string typeName)
        {
            _logger.Trace($"Searching the entry assemblies");
            if (Assembly.GetEntryAssembly() is var assems && assems != null)
            {
                return assems.GetTypes().FirstOrDefault(x => x.FullName == typeName);
            }
            else
            {
                return null!;
            }

        }
        private static Type? SearchTypeInExecutingAssemblies(string typeName)
        {
            _logger.Trace($"Searching the executing assemblies");

            if (Assembly.GetExecutingAssembly() is var assems && assems != null)
            {
                return assems.GetTypes().FirstOrDefault(x => x.FullName == typeName);
            }
            else
            {
                return null!;
            }

        }
        private static Type? SearchTypeInCallingAssemblies(string typeName)
        {
            _logger.Trace($"Searching the calling assemblies");

            if (Assembly.GetCallingAssembly() is var assems && assems != null)
            {
                return assems.GetTypes().FirstOrDefault(x => x.FullName == typeName);
            }
            else
            {
                return null!;
            }

        }

    }
}