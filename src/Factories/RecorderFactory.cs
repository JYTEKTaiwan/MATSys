using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using NLog;

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
                if (section.Exists())
                {
                    _logger.Trace($"Path: {section.Path}");

                    string type = section.GetValue<string>("Type"); //Get the type string of Type in json section
                    string extAssemblyPath = section.GetValue<string>("AssemblyPath"); //Get the assemblypath string of Type in json section

                    _logger.Trace($"Searching for the type named \"{type}\"");

                    var t = TypeParser.SearchType(type, extAssemblyPath);

                    return CreateRecorder(t, section);

                }
                else
                {
                    return CreateRecorder(DefaultType, section);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Create new IRecorder instance (return DefaultInstance if <paramref name="type"/> is not inherited from IRecorder)
        /// </summary>
        /// <param name="type">type of instance</param>
        /// <param name="section">section of configuration</param>
        /// <returns>IRecorder instance</returns>
        private IRecorder CreateRecorder(Type? type, IConfigurationSection section)
        {
            if (type != null)
            {
                var instance = Activator.CreateInstance(type);
                if (instance != null)
                {
                    var obj = (IRecorder)instance;
                    obj.Load(section);
                    return obj;
                }
                else
                {
                    _logger.Debug($"Cannot create instance from \"{type.Name}\", using \"{DefaultType}\" instead");
                    return CreateRecorder(DefaultType, section);
                }
            }
            else
            {
                _logger.Debug($"Type searching failed, using \"{DefaultType}\" instead");
                return CreateRecorder(DefaultType, section);
            }
        }

        private static IRecorder CreateRecorder(Type? type, object args)
        {
            if (type != null)
            {
                var instance = Activator.CreateInstance(type);
                if (instance != null)
                {
                    var obj = (IRecorder)instance;
                    obj.Load(args);
                    return obj;
                }
                else
                {
                    _logger.Debug($"Cannot create instance from \"{type.Name}\", using \"{DefaultType}\" instead");
                    return CreateRecorder(DefaultType, args);
                }
            }
            else
            {
                _logger.Debug($"Type searching failed, using \"{DefaultType}\" instead");
                return CreateRecorder(DefaultType, args);
            }
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

            var t = TypeParser.SearchType(typeString, assemblyPath);

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
        /// Create IRecorder instance statically (return Default instance if <paramref name="t"/> is not inherited from IRecorder)
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