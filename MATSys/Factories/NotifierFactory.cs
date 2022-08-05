using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MATSys.Factories
{
    public sealed class NotifierFactory : INotifierFactory
    {
        private const string sectionKey = "Plugins:Notifiers";
        private const string prefix = "Notifier";
        private readonly static Type DefaultType = typeof(EmptyNotifier);
        private static Lazy<INotifier> _default = new Lazy<INotifier>(() => new EmptyNotifier());
        private static INotifier DefaultInstance => _default.Value;

        public NotifierFactory(IConfiguration config)
        {
            var plugins = config.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();

            //Load plugin assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);
        }

        public INotifier CreateNotifier(IConfigurationSection section)
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

        private INotifier CreateAndLoadInstance(Type defaultType, IConfigurationSection section)
        {
            var obj = (INotifier)Activator.CreateInstance(defaultType)!;
            obj.Load(section);
            return obj;
        }
        public static INotifier CreateNew(string assemblyPath, string typeString, object args)
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
                var obj = (INotifier)Activator.CreateInstance(t)!;
                obj.Load(args);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T CreateNew<T>(object args) where T : INotifier
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            obj.Load(args);
            return obj;
        }
        public static INotifier CreateNew(Type t, object args)
        {
            var obj = Activator.CreateInstance(t) as INotifier;
            obj.Load(args);
            return obj;
        }


    }
}