using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MATSys.Factories
{
    public sealed class TransceiverFactory : ITransceiverFactory
    {
        private const string sectionKey = "Plugins:Transceivers";
        private const string prefix = "Transceiver";
        private readonly static Type DefaultType = typeof(EmptyTransceiver);
        private static Lazy<ITransceiver> _default = new Lazy<ITransceiver>(() => new EmptyTransceiver());
        private static ITransceiver DefaultInstance => _default.Value;

        private readonly IConfiguration _configuration;

        public TransceiverFactory(IConfiguration config)
        {
            _configuration = config;
            var plugins = _configuration.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();

            //Load plugin assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);
        }

        public ITransceiver CreateTransceiver(IConfigurationSection section)
        {
            try
            {
                Type t = DefaultType;
                //check if section in the json configuration exits
                if (section.Exists())
                {
                    var assems = AppDomain.CurrentDomain.GetAssemblies();
                    string type = section.GetValue<string>("Type");
                    if (!string.IsNullOrEmpty(type))
                    {
                        //if key has value, search the type with the default class name. eg. xxx=>xxxDataRecorder
                        foreach (var assem in assems)
                        {
                            var dummy = assem.GetTypes().FirstOrDefault(x => x.Name.ToLower() == $"{type}{prefix}".ToLower());
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
                }
                return CreateAndLoadInstance(t, section);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ITransceiver CreateAndLoadInstance(Type defaultType, IConfigurationSection section)
        {
            var obj = (ITransceiver)Activator.CreateInstance(defaultType)!;
            obj.Load(section);
            return obj;
        }
        public static ITransceiver CreateNew(string assemblyPath, string typeString, object args)
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

                Type t = DefaultType;
                //check if section in the json configuration exits

                if (!string.IsNullOrEmpty(typeString))
                {
                    //if key has value, search the type with the default class name. eg. xxx=>xxxDataRecorder
                    foreach (var assem in assems)
                    {
                        var dummy = assem.GetTypes().FirstOrDefault(x => x.Name.ToLower() == $"{typeString}{prefix}".ToLower());
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
                var obj = (ITransceiver)Activator.CreateInstance(t)!;
                obj.Load(args);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T CreateNew<T>(object args) where T : ITransceiver
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            obj.Load(args);
            return obj;
        }
        public static ITransceiver CreateNew(Type t, object args)
        {
            var obj = Activator.CreateInstance(t) as ITransceiver;
            obj.Load(args);
            return obj;
        }

    }
}