using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MATSys.Factories
{
    public sealed class ModuleFactory : IModuleFactory
    {

        /// <summary>
        /// section key for module assembly reference paths
        /// </summary>
        private const string sectionKey = "MATSys:References:Modules";

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

        private readonly IConfiguration _config;
        private readonly ITransceiverFactory _transceiverFactory;
        private readonly INotifierFactory _notifierFactory;
        private readonly IRecorderFactory _recorderFactory;
        /// <summary>
        /// Ctor for Module factory (dynamically load the assemblies and dependencies from the specified path)
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="tran_factory">Factory for Transceiver</param>
        /// <param name="noti_factory">Factory for Notifier</param>
        /// <param name="rec_factory">Factory for Recorder</param>
        public ModuleFactory(IConfiguration config, ITransceiverFactory tran_factory, INotifierFactory noti_factory, IRecorderFactory rec_factory)
        {
            _config = config;
            _transceiverFactory = tran_factory;
            _notifierFactory = noti_factory;
            _recorderFactory = rec_factory;
            var plugins = _config.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();
            //Load plugin assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);
        }
        /// <summary>
        /// Create IModule instance using specific section in json file
        /// </summary>
        /// <param name="section">specific section in json file</param>
        /// <returns>IModule instance</returns>
        public IModule CreateModule(IConfigurationSection section)
        {
            var _section = section;
            //get the name of the instance
            var name = section.GetSection("Name").Get<string>();
            //get the type string of the instance
            var typeString = section.GetSection("Type").Get<string>();
            
            //Derive the correct Type from AppDomain.CurrentDomain
            Type t = ParseType(typeString);

            //Create Transceiver, Notifier, and Recorder
            var trans = _transceiverFactory.CreateTransceiver(section.GetSection(key_transceiver));
            var noti = _notifierFactory.CreateNotifier(section.GetSection(key_notifier));
            var rec = _recorderFactory.CreateRecorder(section.GetSection(key_recorder));

            //Create instance and return 
            return (IModule)Activator.CreateInstance(t, new object[] { section, trans, noti, rec, name })!;
        }

        /// <summary>
        /// Parse type string into Type from AppDomain.CurrentDomain
        /// </summary>
        /// <param name="typeString"> string of type</param>
        /// <returns></returns>
        public Type ParseType(string typeString)
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            Type t = typeof(object);
            //check if section in the json configuration exits
            if (!string.IsNullOrEmpty(typeString))
            {
                //search all types in all assemblies
                foreach (var assem in assems)
                {
                    var dummy = assem.GetTypes().FirstOrDefault(x => x.Name.ToLower() == $"{typeString}".ToLower());
                    if (dummy == null)
                    {
                        continue;
                    }
                    else
                    {
                        t = dummy;
                        break;
                    }
                }
            }
            return t;
        }
        /// <summary>
        /// Create new Module statically, loaded from dll path
        /// </summary>
        /// <param name="assemblyPath">assembly path</param>
        /// <param name="typeString">string of type</param>
        /// <param name="configuration">configuration object that will be loaded using Load method in IModule</param>
        /// <param name="transceiver">transceiver instance</param>
        /// <param name="notifier">notifier instance</param>
        /// <param name="recorder">recorder instance</param>
        /// <param name="aliasName">Alias name</param>
        /// <returns>IModule instance</returns>
        public static IModule CreateNew(string assemblyPath, string typeString, object configuration, ITransceiver transceiver, INotifier notifier, IRecorder recorder, string aliasName = "")
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = Assembly.LoadFile(Path.GetFullPath(assemblyPath));
            if (assems.FirstOrDefault(x => x.FullName == assembly.FullName) != null)
            {
                //exists
            }
            else
            {
                DependencyLoader.LoadPluginAssemblies(new string[] { assemblyPath });
            }
            try
            {

                assems = AppDomain.CurrentDomain.GetAssemblies();

                Type t = null;
                //check if section in the json configuration exits

                if (!string.IsNullOrEmpty(typeString))
                {
                    foreach (var assem in assems)
                    {
                        var dummy = assem.GetTypes().FirstOrDefault(x => x.Name.ToLower() == $"{typeString}".ToLower());
                        if (dummy == null)
                        {
                            continue;
                        }
                        else
                        {
                            t = dummy;
                            break;
                        }
                    }
                }
                var obj = (IModule)Activator.CreateInstance(t, new object[] { configuration, transceiver, notifier, recorder, aliasName })!;
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Create new Module of generic type statically 
        /// </summary>
        /// <param name="parameter">parameter object</param>
        /// <param name="transceiver">transceiver instance</param>
        /// <param name="notifier">transceiver instance</param>
        /// <param name="recorder">recorder instance</param>
        /// <param name="aliasName">Alias name</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateNew<T>(object parameter, ITransceiver transceiver, INotifier notifier, IRecorder recorder, string aliasName = "") where T : IModule
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { parameter, transceiver, notifier, recorder, aliasName });
        }
        /// <summary>
        /// Create new Module statically. return null if <paramref name="moduleType"/> is not inherited from IModule
        /// </summary>
        /// <param name="moduleType">typeof module</param>
        /// <param name="parameter">parameter object</param>
        /// <param name="transceiver">transceiver instance</param>
        /// <param name="notifier">transceiver instance</param>
        /// <param name="recorder">recorder instance</param>
        /// <param name="aliasName">Alias name</param>
        /// <returns>IModule instance</returns>
        public static IModule CreateNew(Type moduleType, object parameter, ITransceiver transceiver, INotifier notifier, IRecorder recorder, string aliasName = "")
        {
            if (typeof(IModule).IsAssignableFrom(moduleType))
            {
                return (IModule)Activator.CreateInstance(moduleType, new object[] { parameter, transceiver, notifier, recorder, aliasName });
            }
            else
            {
                return null;
            }
        }
    }
}