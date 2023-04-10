using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Diagnostics;
using System.Reflection;
using static System.Collections.Specialized.BitVector32;

namespace MATSys.Factories
{
    /// <summary>
    /// Factory used to create module 
    /// </summary>
    public sealed class ModuleFactory : IModuleFactory
    {

        /// <summary>
        /// section key for recorder in specific section of module
        /// </summary>
        private const string key_recorder = "Recorder";

        /// <summary>
        /// section key for notifier in specific section of module
        /// </summary>
        private const string key_notifier = "Notifier";
        /// <summary>
        /// section key for transceiver in specific section of module
        /// </summary>
        private const string key_transceiver = "Transceiver";

        private readonly ITransceiverFactory _transceiverFactory;
        private readonly INotifierFactory _notifierFactory;
        private readonly IRecorderFactory _recorderFactory;
        private readonly static NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="provider">service provide from host</param>
        public ModuleFactory(IServiceProvider provider)
        {
            _transceiverFactory = provider.GetRequiredService<ITransceiverFactory>();
            _notifierFactory = provider.GetRequiredService<INotifierFactory>();
            _recorderFactory = provider.GetRequiredService<IRecorderFactory>();
        }
        /// <summary>
        /// Create IModule instance using specific section in json file
        /// </summary>
        /// <param name="section">specific section in json file</param>
        /// <returns>IModule instance</returns>
        public IModule CreateModule(IConfigurationSection section)
        {
            try
            {
                _logger.Trace($"Path: {section.Path}");

                string alias = section.GetValue<string>("Alias"); //Get the alias string in json section
                string typeString = section.GetValue<string>("Type"); //Get the type string of Type in json section
                string extAssemblyPath = section.GetValue<string>("AssemblyPath"); //Get the assemblypath string of Type in json section

                _logger.Trace($"Searching for the type named \"{typeString}\"");

                var t = TypeParser.SearchType(typeString, extAssemblyPath);

                if (t == null)
                {
                    throw new InvalidDataException($"Cannot find type {typeString}");
                }
                _logger.Debug($"{t.FullName} is found");

                //Create Transceiver, Notifier, and Recorder
                var trans = _transceiverFactory.CreateTransceiver(section.GetSection(key_transceiver));
                var noti = _notifierFactory.CreateNotifier(section.GetSection(key_notifier));
                var rec = _recorderFactory.CreateRecorder(section.GetSection(key_recorder));
                _logger.Debug($"[{alias}]{typeString} is created with {trans.Alias},{noti.Alias},{rec.Alias}");

                //Create instance and return
                var instance = Activator.CreateInstance(t);
                if (instance != null)
                {
                    var obj = (IModule)instance;
                    obj.Alias = alias;
                    obj.Configure(section);
                    obj.InjectRecorder(rec);
                    obj.InjectNotifier(noti);
                    obj.InjectTransceiver(trans);
                    return obj;
                }
                else
                {
                    throw new NullReferenceException($"Cannot create instance from type '{t.Name}'");
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Create new instance from external dll path
        /// </summary>
        /// <param name="assemblyPath">external dll path</param>
        /// <param name="typeName">name of type</param>
        /// <param name="args">configuration instance</param>
        /// <returns>IModule instance</returns>
        public static IModule CreateNew(string assemblyPath, string typeName, object args)
        {
            var t = TypeParser.SearchType(typeName, assemblyPath);
            if (t != null)
            {
                //Create instance and return
                var instance = Activator.CreateInstance(t);
                if (instance != null)
                {
                    var obj = (IModule)instance;
                    obj.Configure(args);
                    return obj;
                }
                else
                {
                    throw new NullReferenceException($"Cannot create instance from type '{typeName}'");
                }
            }
            else
            {
                throw new NullReferenceException($"\"{typeName}\" is not found");
            }
            
        }

        /// <summary>
        /// Create new instance from known type
        /// </summary>
        /// <typeparam name="T">Type of the IModule implementation</typeparam>
        /// <param name="args">configuration instance</param>
        /// <returns>IModule instance</returns>
        public static IModule CreateNew<T>(object args) where T : IModule
        {
            var t = typeof(T);
            //Create instance and return
            var instance = Activator.CreateInstance(t);
            if (instance != null)
            {
                var obj = (IModule)instance;
                obj.Configure(args);
                return obj;
            }
            else
            {
                throw new NullReferenceException($"Cannot create instance from type '{t.Name}'");
            }
        }

    }
}