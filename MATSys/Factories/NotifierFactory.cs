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

        private readonly static Type DefaultType = typeof(EmptyNotifier);
        private readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Create new INotifier instance using specified section of configuration
        /// </summary>
        /// <param name="section">section of configuration object</param>
        /// <returns>INotifier instance</returns>
        public INotifier CreateNotifier(IConfigurationSection section)
        {
            try
            {
                var t = DefaultType;
                if (section.Exists())
                {
                    _logger.Trace($"Path: {section.Path}");

                    string type = section.GetValue<string>("Type"); //Get the type string of Type in json section
                    string extAssemblyPath = section.GetValue<string>("AssemblyPath"); //Get the assemblypath string of Type in json section

                    _logger.Trace($"Searching for the type named \"{type}\"");

                    t = SearchType(type, extAssemblyPath);
                }
             

                _logger.Debug($"{t.Name} is used");

                return CreateNotifier(t, section);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Type SearchType(string type, string extAssemPath)
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

        private static Type StaticSearchType(string type, string extAssemPath)
        {
            Type t = DefaultType;

            if (!string.IsNullOrEmpty(type)) // return EmptyNotifier if type is empty or null
            {
                // 1.  Look up the existed assemlies in GAC
                // 1.y if existed, get the type directly and overrider the variable t
                // 1.n if not, dynamically load the assembly from the section "AssemblyPath" and search for the type

                var dummy = Type.GetType(Assembly.CreateQualifiedName(type, type));
                if (dummy != null)
                {
                    t = dummy;
                }
                else
                {

                    //load the assembly from external path
                    var assem = DependencyLoader.LoadPluginAssemblies(new string[] { extAssemPath }).First();

                    dummy = assem.GetType(type);
                    if (dummy != null)
                    {
                        t = dummy;
                    }

                }
            }
            return t;
        }

        /// <summary>
        /// Create new INotifier instance (return DefaultInstance if <paramref name="type"/> is not inherited from INotifier)
        /// </summary>
        /// <param name="type">type of instance</param>
        /// <param name="section">section of configuration</param>
        /// <returns>INotifier instance</returns>
        private INotifier CreateNotifier(Type type, IConfigurationSection section)
        {
            _logger.Trace($"Creating instance of {type.Name}");
            if (typeof(INotifier).IsAssignableFrom(type))
            {
                var obj = (INotifier)Activator.CreateInstance(type)!;
                _logger.Debug($"Instance is created [{obj.GetHashCode()}]{type.Name}");
                _logger.Trace($"Loading the configuration from {section.Path}");
                obj.Load(section);
                return obj;
            }
            else
                return CreateNotifier(DefaultType, section);
        }

        private static INotifier CreateNotifier(Type type, object args)
        {
            if (typeof(INotifier).IsAssignableFrom(type))
            {
                var obj = (INotifier)Activator.CreateInstance(type)!;
                obj.Load(args);
                return obj;
            }
            else
                return CreateNotifier(DefaultType, args);
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

            Type t = StaticSearchType(typeString, assemblyPath);

            return CreateNotifier(t, args);
        }
        /// <summary>
        /// Create <typeparamref name="T"/> instance statically
        /// </summary>
        /// <param name="args">parameter object</param>
        /// <typeparam name="T">type inherited from INotifier</typeparam>
        /// <returns><typeparamref name="T"/> instance</returns>
        public static T CreateNew<T>(object args) where T : INotifier
        {
            return (T)CreateNotifier(typeof(T), args)!;
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
                return CreateNotifier(t, args);
            }
            else

                return CreateNotifier(DefaultType, args);
        }


    }
}