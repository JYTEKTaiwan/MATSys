using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MATSys.Factories
{
    public sealed class RecorderFactory : IRecorderFactory
    {

        /// <summary>
        /// searching prefix string
        /// </summary>
        private const string prefix = "Recorder";
        /// <summary>
        /// section key for recorder assembly reference paths
        /// </summary>
        private const string sectionKey = "MATSys:References:Recorders";
        private readonly static Type DefaultType = typeof(EmptyRecorder);
        private static Lazy<IRecorder> _default = new Lazy<IRecorder>(() => new EmptyRecorder());
        private static IRecorder DefaultInstance => _default.Value;

        /// <summary>
        /// ctor of recorder factory (dynamically load the assemblies and dependencies from the specified path)
        /// </summary>
        /// <param name="config">configuration instance</param>
        public RecorderFactory(IConfiguration config)
        {
            // list the Recorder reference paths in the json file
            var plugins = config.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();

            //Load plugin assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);
        }
        /// <summary>
        /// Create new IRecorder instance using specified section of configuration
        /// </summary>
        /// <param name="section">section of configuration object</param>
        /// <returns>IRecorder instance</returns>
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
                        //if key has value, search the type with the default class name. eg. xxx=>xxxRecorder
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
                return CreateRecorder(t, section);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Create new IRecorder instance (return DefaultInstance if <paramref name="defaultType"/> is not inherited from IRecorder)
        /// </summary>
        /// <param name="defaultType">type of instance</param>
        /// <param name="section">section of configuration</param>
        /// <returns>IRecorder instance</returns>
        private IRecorder CreateRecorder(Type defaultType, IConfigurationSection section)
        {
            if (typeof(IRecorder).IsAssignableFrom(defaultType))
            {
                var obj = (IRecorder)Activator.CreateInstance(defaultType)!;
                obj.Load(section);
                return obj;
            }
            else
                return DefaultInstance;
        }
        /// <summary>
        /// Create new IRecorder instance by loaded from file (return Default if type is not found) 
        /// </summary>
        /// <param name="assemblyPath">assembly path</param>
        /// <param name="typeString">string of type</param>
        /// <param name="args">parameter instance</param>
        /// <returns>IRecorder instance</returns>
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
                    //if key has value, search the type with the default class name. eg. xxx=>xxxRecorder
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
                obj.Load(args);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Create <typeparamref name="T"/> instance statically
        /// </summary>
        /// <param name="args">parameter object</param>
        /// <typeparam name="T">type inherited from IRecorder</typeparam>
        /// <returns><typeparamref name="T"/> instance</returns>
        public static T CreateNew<T>(object args) where T : IRecorder
        {
            var obj = (T)Activator.CreateInstance(typeof(T))!;
            obj?.Load(args);
            return obj!;
        }
        /// <summary>
        /// Create IRecorder instance statically (return Default instance if <paramref name="T"/> is not inherited from IRecorder)
        /// </summary>
        /// <param name="t">type</param>
        /// <param name="args">parameter object</param>
        /// <returns>IRecorder instance</returns>
        public static IRecorder CreateNew(Type t, object args)
        {
            if (typeof(IRecorder).IsAssignableFrom(t))
            {
                var obj = Activator.CreateInstance(t) as IRecorder;
                obj?.Load(args);
                return obj!;
            }
            else
           
                return DefaultInstance;
        }

    }
}