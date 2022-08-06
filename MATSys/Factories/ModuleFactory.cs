using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MATSys.Factories
{
    public sealed class ModuleFactory : IModuleFactory
    {
        private const string sectionKey = "MATSys:References:Modules";
        private const string key_module = "Modules";
        private const string key_recorder = "Recorder";
        private const string key_notifier = "Notifier";
        private const string key_transceiver = "Transceiver";

        private readonly IConfiguration _config;
        private readonly ITransceiverFactory _transceiverFactory;
        private readonly INotifierFactory _notifierFactory;
        private readonly IRecorderFactory _recorderFactory;
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
        public IModule CreateDevice(IConfigurationSection section)
        {
            var _section = section;
            var name = section.GetSection("Name").Get<string>();
            var typeString = section.GetSection("Type").Get<string>();

            Type t = ParseType(typeString);
            var trans = _transceiverFactory.CreateTransceiver(section.GetSection(key_transceiver));
            var noti = _notifierFactory.CreateNotifier(section.GetSection(key_notifier));
            var rec = _recorderFactory.CreateRecorder(section.GetSection(key_recorder));
            return (IModule)Activator.CreateInstance(t,new  object[] { section, trans, noti, rec, name })!;
        }
        public Type ParseType(string typeString)
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            Type t = typeof(object);
            //check if section in the json configuration exits
            if (!string.IsNullOrEmpty(typeString))
            {
                //if key has value, search the type with the default class name. eg. xxx=>xxxRecorder
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
        public static IModule CreateNew(string assemblyPath, string typeString, object configuration, ITransceiver server, INotifier bus, IRecorder recorder, string configurationKey = "")
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
                var obj = (IModule)Activator.CreateInstance(t, new object[] { configuration, server, bus, recorder, configurationKey })!;
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static T CreateNew<T>(object parameter, ITransceiver transceiver, INotifier notifier, IRecorder recorder, string configurationKey = "") where T : IModule
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { parameter, transceiver, notifier, recorder, configurationKey });
        }
        public static IModule CreateNew(Type moduleType, object parameter, ITransceiver transceiver, INotifier notifier, IRecorder recorder, string configurationKey = "")
        {
            if (typeof(IModule).IsAssignableFrom(moduleType))
            {
                return (IModule)Activator.CreateInstance(moduleType, new object[] { parameter, transceiver, notifier, recorder, configurationKey });
            }
            else
            {
                return null;
            }
        }
    }
}