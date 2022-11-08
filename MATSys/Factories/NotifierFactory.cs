using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Reflection;
using System.Text.Json;

namespace MATSys.Factories
{
    /// <summary>
    /// Factory used to create Notifier
    /// </summary>
    public sealed class NotifierFactory : INotifierFactory
    {
        /// <summary>
        /// section key for notifier assembly reference paths
        /// </summary>
        private const string sectionKey = "MATSys:References:Notifiers";

        /// <summary>
        /// searching prefix string
        /// </summary>
        private const string prefix = "Notifier";
        private readonly static Type DefaultType = typeof(EmptyNotifier);
        private static Lazy<INotifier> _default = new Lazy<INotifier>(() => new EmptyNotifier());
        private static INotifier DefaultInstance => _default.Value;
        private readonly NLog.ILogger _logger;


        /// <summary>
        /// ctor of notifier factory (dynamically load the assemblies and dependencies from the specified path)
        /// </summary>
        /// <param name="config">configuration instance</param>
        public NotifierFactory(IConfiguration config)
        {
            _logger = LogManager.GetCurrentClassLogger();
            // list the notifier reference paths in the json file
            var plugins = config.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();
            _logger.Debug($"External references: {JsonSerializer.Serialize(plugins)}");

            //Load plugin assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);
            _logger.Info($"{plugins.Length} External references is/are loaded");

        }

        /// <summary>
        /// Create new INotifier instance using specified section of configuration
        /// </summary>
        /// <param name="section">section of configuration object</param>
        /// <returns>INotifier instance</returns>
        public INotifier CreateNotifier(IConfigurationSection section)
        {
            try
            {
                _logger.Trace($"Path: {section.Path}");
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
                _logger.Debug($"{t.Name} is used");
                return CreateNotifier(t, section);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Create new INotifier instance (return DefaultInstance if <paramref name="defaultType"/> is not inherited from INotifier)
        /// </summary>
        /// <param name="defaultType">type of instance</param>
        /// <param name="section">section of configuration</param>
        /// <returns>INotifier instance</returns>
        private INotifier CreateNotifier(Type defaultType, IConfigurationSection section)
        {
            if (typeof(INotifier).IsAssignableFrom(defaultType))
            {
                var obj = (INotifier)Activator.CreateInstance(defaultType)!;
                obj.Load(section);
                return obj;
            }
            else
                return DefaultInstance;

        }
        /// <summary>
        /// Create new INotifier instance by loaded from file (return Default if type is not found) 
        /// </summary>
        /// <param name="assemblyPath">assembly path</param>
        /// <param name="typeString">string of type</param>
        /// <param name="args">parameter instance</param>
        /// <returns>INotifier instance</returns>
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
                var obj = (INotifier)Activator.CreateInstance(t)!;
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
        /// <typeparam name="T">type inherited from INotifier</typeparam>
        /// <returns><typeparamref name="T"/> instance</returns>
        public static T CreateNew<T>(object args) where T : INotifier
        {
            var obj = (T)Activator.CreateInstance(typeof(T))!;
            obj?.Load(args);
            return obj!;
        }
        /// <summary>
        /// Create INotifier instance statically (return Default instance if <paramref name="T"/> is not inherited from INotifier)
        /// </summary>
        /// <param name="t">type</param>
        /// <param name="args">parameter object</param>
        /// <returns>INotifier instance</returns>
        public static INotifier CreateNew(Type t, object args)
        {
            if (typeof(INotifier).IsAssignableFrom(t))
            {
                var obj = Activator.CreateInstance(t) as INotifier;
                obj?.Load(args);
                return obj!;
            }
            else
            {
                return DefaultInstance;
            }

        }


    }
}