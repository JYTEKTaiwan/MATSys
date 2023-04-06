using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Reflection;

namespace MATSys.Factories
{
    /// <summary>
    /// Factory used to create recorder
    /// </summary>
    public sealed class RecorderFactory : IRecorderFactory
    {

        private readonly static Type DefaultType = typeof(EmptyRecorder);
        private static NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

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
                if (section.Exists())
                {
                    _logger.Trace($"Path: {section.Path}");

                    string type = section.GetValue<string>("Type"); //Get the type string of Type in json section
                    string extAssemblyPath = section.GetValue<string>("AssemblyPath"); //Get the assemblypath string of Type in json section

                    _logger.Trace($"Searching for the type named \"{type}\"");

                    t = SearchType(type, extAssemblyPath);

                }
                _logger.Debug($"{t.Name} is used");

                return CreateRecorder(t, section);
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

        /// <summary>
        /// Create new IRecorder instance (return DefaultInstance if <paramref name="type"/> is not inherited from IRecorder)
        /// </summary>
        /// <param name="type">type of instance</param>
        /// <param name="section">section of configuration</param>
        /// <returns>IRecorder instance</returns>
        private IRecorder CreateRecorder(Type type, IConfigurationSection section)
        {
            _logger.Trace($"Creating instance of {type.Name}");
            if (typeof(IRecorder).IsAssignableFrom(type))
            {
                var obj = (IRecorder)Activator.CreateInstance(type)!;
                _logger.Debug($"Instance is created [{obj.GetHashCode()}]{type.Name}");
                _logger.Trace($"Loading the configuration from {section.Path}");
                obj.Load(section);
                return obj;
            }
            else
                return CreateRecorder(DefaultType, section);
        }

        private static IRecorder CreateRecorder(Type type, object args)
        {
            if (typeof(IRecorder).IsAssignableFrom(type))
            {
                var obj = (IRecorder)Activator.CreateInstance(type)!;
                obj.Load(args);
                return obj;
            }
            else
                return CreateRecorder(DefaultType, args);
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

            Type t = SearchType(typeString, assemblyPath);

            return CreateRecorder(t, args);
        }
        /// <summary>
        /// Create <typeparamref name="T"/> instance statically
        /// </summary>
        /// <param name="args">parameter object</param>
        /// <typeparam name="T">type inherited from IRecorder</typeparam>
        /// <returns><typeparamref name="T"/> instance</returns>
        public static T CreateNew<T>(object args) where T : IRecorder
        {
            return (T)CreateRecorder(typeof(T), args)!;
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
                return CreateRecorder(t, args);
            }
            else

                return CreateRecorder(DefaultType, args);
        }

    }
}