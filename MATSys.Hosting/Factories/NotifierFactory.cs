using MATSys.Plugins;
using MATSys.Utilities;
using Microsoft.Extensions.Configuration;
using NLog;

namespace MATSys.Factories
{
    /// <summary>
    /// Factory used to create Notifier
    /// </summary>
    public sealed class NotifierFactory : INotifierFactory
    {

        private readonly static Type DefaultType = typeof(EmptyNotifier);
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Create new INotifier instance using specified section of configuration. Will return EmptyNotifier if any exception occured or not type is found
        /// </summary>
        /// <param name="section">section of configuration object</param>
        /// <returns>INotifier instance</returns>
        public INotifier CreateNotifier(IConfigurationSection section)
        {
            try
            {
                if (!section.Exists()) return CreateNotifier(DefaultType, section);

                _logger.Trace($"Path: {section.Path}");

                string typeString = section.GetValue<string>("Type")!; //Get the type string of Type in json section
                string extAssemblyPath = section.GetValue<string>("AssemblyPath")!; //Get the assemblypath string of Type in json section

                _logger.Trace($"Searching for the type named \"{typeString}\"");

                var t = TypeParser.SearchType(typeString, extAssemblyPath);
                return CreateNotifier(t == null ? DefaultType : t, section);


            }
            catch (Exception ex)
            {
                _logger.Warn($"Exception occured when creating recorder. Use EmptyRecorder instead.\"{ex.Message}\"");
                return CreateNotifier(DefaultType, section);
            }
        }


        private INotifier CreateNotifier(Type? type, IConfigurationSection section)
        {
            if (type != null)
            {
                var instance = Activator.CreateInstance(type);
                if (instance != null)
                {
                    var obj = (INotifier)instance;
                    obj.Load(section);
                    return obj;
                }
                else
                {
                    _logger.Debug($"Cannot create instance from \"{type.Name}\", using \"{DefaultType}\" instead");
                    return CreateNotifier(DefaultType, section);
                }
            }
            else
            {
                _logger.Debug($"Type searching failed, using \"{DefaultType}\" instead");
                return CreateNotifier(DefaultType, section);
            }
        }

        private static INotifier CreateNotifier(Type? type, object args)
        {
            if (type != null)
            {
                var instance = Activator.CreateInstance(type);
                if (instance != null)
                {
                    var obj = (INotifier)instance;
                    obj.Load(args);
                    return obj;
                }
                else
                {
                    _logger.Debug($"Cannot create instance from \"{type.Name}\", using \"{DefaultType}\" instead");
                    return CreateNotifier(DefaultType, args);
                }
            }
            else
            {
                _logger.Debug($"Type searching failed, using \"{DefaultType}\" instead");
                return CreateNotifier(DefaultType, args);
            }
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

            var t = TypeParser.SearchType(typeString, assemblyPath);

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
        /// Create INotifier instance statically (return Default instance if <paramref name="t"/> is not inherited from INotifier)
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