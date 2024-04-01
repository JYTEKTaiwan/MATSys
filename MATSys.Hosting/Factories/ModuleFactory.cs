using MATSys.Hosting;
using MATSys.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Data;

namespace MATSys.Factories
{
    /// <summary>
    /// Factory used to create module 
    /// </summary>
    public sealed class ModuleFactory : IModuleFactory
    {
        private const string m_key_modConfigSection = "MATSys:Modules";
        private const string m_key_modAlias = "Alias";
        private const string m_key_modType = "Type";
        private const string m_key_modextPath = "AssemblyPath";
        private const string m_key_recorder = "Recorder";
        private const string m_key_notifier = "Notifier";
        private const string m_key_transceiver = "Transceiver";

        private readonly ITransceiverFactory _transceiverFactory;
        private readonly INotifierFactory _notifierFactory;
        private readonly IRecorderFactory _recorderFactory;
        private readonly static NLog.ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IServiceProvider _serviceprovider;
        private readonly Dictionary<string, IConfigurationSection> _modConfigurations;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="provider">service provide from host</param>
        public ModuleFactory(IServiceProvider provider)
        {
            _serviceprovider = provider;
            _modConfigurations = provider.GetConfigurationSection(m_key_modConfigSection).GetChildren().ToDictionary(x => x[m_key_modAlias]!);
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

                string alias = section.GetValue<string>(m_key_modAlias)!; //Get the alias string in json section
                if (alias == null) throw new NoNullAllowedException("Alias property cannot be null in configuration section");

                string typeString = section.GetValue<string>(m_key_modType)!; //Get the type string of Type in json section
                if (typeString == null) throw new NoNullAllowedException("Type property cannot be null in configuration section");

                string extAssemblyPath = section.GetValue<string>(m_key_modextPath)!; //Get the assemblypath string of Type in json section


                _logger.Trace($"Searching for the type named \"{typeString}\"");

                var t = TypeParser.SearchType(typeString, extAssemblyPath);
                if (t == null) throw new InvalidDataException($"Cannot find type {typeString}");

                _logger.Debug($"{t.FullName} is found");

                //Create Transceiver, Notifier, and Recorder
                var trans = _transceiverFactory.CreateTransceiver(section.GetSection(m_key_transceiver));
                var noti = _notifierFactory.CreateNotifier(section.GetSection(m_key_notifier));
                var rec = _recorderFactory.CreateRecorder(section.GetSection(m_key_recorder));
                _logger.Debug($"[{alias}]{typeString} is created with {trans.Alias},{noti.Alias},{rec.Alias}");

                //Create instance and return
                var instance = Activator.CreateInstance(t);
                if (instance != null)
                {
                    var obj = (IModule)instance;
                    obj.Alias = alias;
                    obj.SetProvider(_serviceprovider);
                    obj.Configure(section);
                    obj.InjectPlugin(rec);
                    obj.InjectPlugin(noti);
                    obj.InjectPlugin(trans);
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
        public IModule CreateModule(string alias)
        {
            try
            {
                var section = _modConfigurations[alias];
                return CreateModule(section);
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