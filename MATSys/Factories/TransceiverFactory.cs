using MATSys.Plugins;
using MATSys.Utilities;

namespace MATSys.Factories
{
    /// <summary>
    /// Factory used to create Transceiver
    /// </summary>
    public sealed class TransceiverFactory : ITransceiverFactory
    {

        private readonly static Type DefaultType = typeof(EmptyTransceiver);
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Create new ITransceiver instance using specified section of configuration. Will return EmptyTransceiver if any exception occured or not type is found
        /// </summary>
        /// <param name="section">section of configuration object</param>
        /// <returns>ITransceiver instance</returns>
        public ITransceiver CreateTransceiver(IConfigurationSection section)
        {
            try
            {
                if (!section.Exists()) return CreateTransceiver(DefaultType, section);

                _logger.Trace($"Path: {section.Path}");

                string typeString = section.GetValue<string>("Type")!; //Get the type string of Type in json section                  

                string extAssemblyPath = section.GetValue<string>("AssemblyPath")!; //Get the assemblypath string of Type in json section

                _logger.Trace($"Searching for the type named \"{typeString}\"");

                var t = TypeParser.SearchType(typeString, extAssemblyPath);

                return CreateTransceiver(t == null ? DefaultType : t, section);
            }
            catch (Exception ex)
            {
                _logger.Warn($"Exception occured when creating recorder. Use EmptyRecorder instead.\"{ex.Message}\"");
                return CreateTransceiver(DefaultType, section);
            }
        }


        /// <summary>
        /// Create new ITransceiver instance (return DefaultInstance if <paramref name="type"/> is not inherited from ITransceiver)
        /// </summary>
        /// <param name="type">type of instance</param>
        /// <param name="section">section of configuration</param>
        /// <returns>ITransceiver instance</returns>
        private ITransceiver CreateTransceiver(Type? type, IConfigurationSection section)
        {
            if (type != null)
            {
                var instance = Activator.CreateInstance(type);
                if (instance != null)
                {
                    var obj = (ITransceiver)instance;
                    obj.Load(section);
                    return obj;
                }
                else
                {
                    _logger.Debug($"Cannot create instance from \"{type.Name}\", using \"{DefaultType}\" instead");
                    return CreateTransceiver(DefaultType, section);
                }
            }
            else
            {
                _logger.Debug($"Type searching failed, using \"{DefaultType}\" instead");
                return CreateTransceiver(DefaultType, section);
            }
        }

        private static ITransceiver CreateTransceiver(Type? type, object args)
        {
            if (type != null)
            {
                var instance = Activator.CreateInstance(type);
                if (instance != null)
                {
                    var obj = (ITransceiver)instance;
                    obj.Load(args);
                    return obj;
                }
                else
                {
                    _logger.Debug($"Cannot create instance from \"{type.Name}\", using \"{DefaultType}\" instead");
                    return CreateTransceiver(DefaultType, args);
                }
            }
            else
            {
                _logger.Debug($"Type searching failed, using \"{DefaultType}\" instead");
                return CreateTransceiver(DefaultType, args);
            }
        }

        /// <summary>
        /// Create new ITransceiver instance by loaded from file (return Default if type is not found) 
        /// </summary>
        /// <param name="assemblyPath">assembly path</param>
        /// <param name="typeString">string of type</param>
        /// <param name="args">parameter instance</param>
        /// <returns>ITransceiver instance</returns>
        public static ITransceiver CreateNew(string assemblyPath, string typeString, object args)
        {

            var t = TypeParser.SearchType(typeString, assemblyPath);

            return CreateTransceiver(t, args);
        }
        /// <summary>
        /// Create <typeparamref name="T"/> instance statically
        /// </summary>
        /// <param name="args">parameter object</param>
        /// <typeparam name="T">type inherited from ITransceiver</typeparam>
        /// <returns><typeparamref name="T"/> instance</returns>
        public static T CreateNew<T>(object args) where T : ITransceiver
        {
            return (T)CreateTransceiver(typeof(T), args)!;
        }
        /// <summary>
        /// Create ITransceiver instance statically (return Default instance if <paramref name="t"/> is not inherited from ITransceiver)
        /// </summary>
        /// <param name="t">type</param>
        /// <param name="args">parameter object</param>
        /// <returns>ITransceiver instance</returns>
        public static ITransceiver CreateNew(Type t, object args)
        {
            if (typeof(ITransceiver).IsAssignableFrom(t))
            {
                return CreateTransceiver(t, args);
            }
            else

                return CreateTransceiver(DefaultType, args);
        }

    }
}