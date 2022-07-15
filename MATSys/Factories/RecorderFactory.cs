using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MATSys.Factories
{
    public sealed class RecorderFactory : IRecorderFactory
    {
        private const string prefix = "Recorder";
        private const string sectionKey = "Plugins:Recorders";
        private readonly static Type DefaultType = typeof(EmptyRecorder);
        private static IRecorder DefaultInstance => new EmptyRecorder();
        private readonly IConfiguration _configuration;

        public RecorderFactory(IConfiguration config)
        {
            _configuration = config;
            var plugins = _configuration.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();

            //Load plugin assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);
        }

        public IRecorder CreateRecorder(IConfigurationSection section)
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

        private IRecorder CreateAndLoadInstance(Type type, IConfigurationSection section)
        {
            var obj = (IRecorder)Activator.CreateInstance(type)!;
            obj.Load(section);
            return obj;
        }

        public static IRecorder CreateNew(string assemblyPath, string typeString, object args)
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
                var obj = (IRecorder)Activator.CreateInstance(t)!;
                obj.LoadFromObject(args);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}