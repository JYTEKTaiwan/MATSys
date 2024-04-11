using MATSys.Factories;
using MATSys.Utilities;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Hosting
{
    public sealed class ModuleResolver
    {

        private const string m_key_modConfigSection = "MATSys:Modules";
        private const string m_key_modAlias = "Alias";
        private const string m_key_modType = "Type";
        private const string m_key_modextPath = "AssemblyPath";
        private const string m_key_recorder = "Recorder";
        private const string m_key_notifier = "Notifier";
        private const string m_key_transceiver = "Transceiver";

        private readonly Dictionary<string, IConfigurationSection> _modConfigurations;
        private Dictionary<string, IModule> _livingModules = new Dictionary<string, IModule>();
        private readonly IServiceProvider _provider;
        private readonly ITransceiverFactory _transceiverFactory;
        private readonly INotifierFactory _notifierFactory;
        private readonly IRecorderFactory _recorderFactory;
        
        private object _sync = new object();
        private string _key;
        public string SelectedKey
        {
            get
            {
                lock (_sync)
                {
                    return _key;
                }
            }
            set
            {
                lock (_sync)
                {
                    _key = value;
                }
            }
        }
        public IModule[] ExistingModules => _livingModules.Values.ToArray();
        public ModuleResolver(IServiceProvider provider, ITransceiverFactory tran, INotifierFactory noti, IRecorderFactory rec)
        {
            _provider = provider;
            _modConfigurations = provider.GetConfigurationSection(m_key_modConfigSection).GetChildren().ToDictionary(x => x[m_key_modAlias]!);
            _transceiverFactory = tran;
            _notifierFactory = noti;
            _recorderFactory = rec;

        }
        public IModule GetSelectedModule()
        {
            lock (_sync)
            {
                if (_livingModules.ContainsKey(_key))
                {
                    return _livingModules[SelectedKey];
                }
                else
                {
                    var section = _modConfigurations[_key];
                    var mod = CreateModule(section);
                    _livingModules.Add(_key, mod);
                    return mod;
                }


            }

        }

        public IModule CreateModule(IConfigurationSection section)
        {
            try
            {
                string alias = section.GetValue<string>(m_key_modAlias)!; //Get the alias string in json section
                if (alias == null) throw new NoNullAllowedException("Alias property cannot be null in configuration section");

                string typeString = section.GetValue<string>(m_key_modType)!; //Get the type string of Type in json section
                if (typeString == null) throw new NoNullAllowedException("Type property cannot be null in configuration section");

                string extAssemblyPath = section.GetValue<string>(m_key_modextPath)!; //Get the assemblypath string of Type in json section

                var t = TypeParser.SearchType(typeString, extAssemblyPath);
                if (t == null) throw new InvalidDataException($"Cannot find type {typeString}");

                //Create Transceiver, Notifier, and Recorder
                var trans = _transceiverFactory.CreateTransceiver(section.GetSection(m_key_transceiver));
                var noti = _notifierFactory.CreateNotifier(section.GetSection(m_key_notifier));
                var rec = _recorderFactory.CreateRecorder(section.GetSection(m_key_recorder));

                
                //Create instance and return
                var instance = Activator.CreateInstance(t);
                
                if (instance != null)
                {
                    var obj = (IModule)instance;
                    obj.Alias = alias;
                    obj.SetProvider(_provider);
                    section.Bind(obj.Configuration);
                    obj.Configure();
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
        public IModule CreateModule(string key)
        {
            try
            {
                var section = _modConfigurations[key];

                string alias = section.GetValue<string>(m_key_modAlias)!; //Get the alias string in json section
                if (alias == null) throw new NoNullAllowedException("Alias property cannot be null in configuration section");

                string typeString = section.GetValue<string>(m_key_modType)!; //Get the type string of Type in json section
                if (typeString == null) throw new NoNullAllowedException("Type property cannot be null in configuration section");

                string extAssemblyPath = section.GetValue<string>(m_key_modextPath)!; //Get the assemblypath string of Type in json section


                var t = TypeParser.SearchType(typeString, extAssemblyPath);
                if (t == null) throw new InvalidDataException($"Cannot find type {typeString}");


                //Create Transceiver, Notifier, and Recorder
                var trans = _transceiverFactory.CreateTransceiver(section.GetSection(m_key_transceiver));
                var noti = _notifierFactory.CreateNotifier(section.GetSection(m_key_notifier));
                var rec = _recorderFactory.CreateRecorder(section.GetSection(m_key_recorder));

                //Create instance and return
                var instance = Activator.CreateInstance(t);
                if (instance != null)
                {
                    var obj = (IModule)instance;
                    obj.Alias = alias;
                    obj.SetProvider(_provider);
                    section.Bind(obj.Configuration);
                    obj.Configure();
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

    }

}
