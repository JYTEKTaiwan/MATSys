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

        /// <summary>
        /// Create module without transceiver, recorder and notifier
        /// </summary>
        /// <param name="provider">Service provider</param>
        /// <param name="sectionKey">Key of the section</param>
        /// <returns>IModule</returns>
        /// <exception cref="KeyNotFoundException">sectino is not found</exception>
        /// <exception cref="NoNullAllowedException">"Alias" or "Type" property is null</exception>
        /// <exception cref="InvalidDataException">Cannot property parse the type info</exception>
        /// <exception cref="NullReferenceException">Error when activating the instance</exception>
        public static IModule Create(IServiceProvider provider, string sectionKey)
        {
            var _transceiverFactory = (ITransceiverFactory)provider.GetService(typeof(ITransceiverFactory));
            var _notifierFactory = (INotifierFactory)provider.GetService(typeof(INotifierFactory));
            var _recorderFactory = (IRecorderFactory)provider.GetService(typeof(IRecorderFactory));

            var section = provider.GetConfigurationSection(sectionKey);
            if (!section.Exists()) throw new KeyNotFoundException($"Cannot find section key named \"{sectionKey}\"");

            string alias = section.GetValue<string>(m_key_modAlias)!; //Get the alias string in json section
            if (alias == null) throw new NoNullAllowedException("Alias property cannot be null in configuration section");

            string typeString = section.GetValue<string>(m_key_modType)!; //Get the type string of Type in json section
            if (typeString == null) throw new NoNullAllowedException("Type property cannot be null in configuration section");

            string extAssemblyPath = section.GetValue<string>(m_key_modextPath)!; //Get the assemblypath string of Type in json section

            var t = TypeParser.SearchType(typeString, extAssemblyPath);
            if (t == null) throw new InvalidDataException($"Cannot find type {typeString}");

            //Create instance and return
            var instance = Activator.CreateInstance(t);

            //Create Transceiver, Notifier, and Recorder
            var trans = _transceiverFactory == null ? new EmptyTransceiver() :
                _transceiverFactory.CreateTransceiver(section.GetSection(m_key_transceiver));
            var noti = _notifierFactory == null ? new EmptyNotifier() :
                _notifierFactory.CreateNotifier(section.GetSection(m_key_notifier));
            var rec = _recorderFactory == null ? new EmptyRecorder() :
                _recorderFactory.CreateRecorder(section.GetSection(m_key_recorder));

            if (instance != null)
            {
                var obj = (IModule)instance;
                obj.Alias = alias;
                obj.SetProvider(provider);
                section.Bind(obj.Configuration);
                obj.Configure();
                obj.InjectPlugin(trans);
                obj.InjectPlugin(noti);
                obj.InjectPlugin(rec);
                return obj;
            }
            else
            {
                throw new NullReferenceException($"Cannot create instance from type '{t.Name}'");
            }

        }
        /// <summary>
        /// Create module without transceiver, recorder and notifier
        /// </summary>
        /// <param name="provider">Service provider</param>
        /// <param name="section">section data</param>
        /// <returns>IModule</returns>
        /// <exception cref="KeyNotFoundException">sectino is not found</exception>
        /// <exception cref="NoNullAllowedException">"Alias" or "Type" property is null</exception>
        /// <exception cref="InvalidDataException">Cannot property parse the type info</exception>
        /// <exception cref="NullReferenceException">Error when activating the instance</exception>
        public static IModule Create(IServiceProvider provider, IConfigurationSection section)
        {
            var _transceiverFactory = (ITransceiverFactory)provider.GetService(typeof(ITransceiverFactory));
            var _notifierFactory = (INotifierFactory)provider.GetService(typeof(INotifierFactory));
            var _recorderFactory = (IRecorderFactory)provider.GetService(typeof(IRecorderFactory));

            string alias = section.GetValue<string>(m_key_modAlias)!; //Get the alias string in json section
            if (alias == null) throw new NoNullAllowedException("Alias property cannot be null in configuration section");

            string typeString = section.GetValue<string>(m_key_modType)!; //Get the type string of Type in json section
            if (typeString == null) throw new NoNullAllowedException("Type property cannot be null in configuration section");

            string extAssemblyPath = section.GetValue<string>(m_key_modextPath)!; //Get the assemblypath string of Type in json section

            var t = TypeParser.SearchType(typeString, extAssemblyPath);
            if (t == null) throw new InvalidDataException($"Cannot find type {typeString}");

            //Create instance and return
            var instance = Activator.CreateInstance(t);

            //Create Transceiver, Notifier, and Recorder
            var trans = _transceiverFactory == null ? new EmptyTransceiver() :
                _transceiverFactory.CreateTransceiver(section.GetSection(m_key_transceiver));
            var noti = _notifierFactory == null ? new EmptyNotifier() :
                _notifierFactory.CreateNotifier(section.GetSection(m_key_notifier));
            var rec = _recorderFactory == null ? new EmptyRecorder() :
                _recorderFactory.CreateRecorder(section.GetSection(m_key_recorder));

            if (instance != null)
            {
                var obj = (IModule)instance;
                obj.Alias = alias;
                obj.SetProvider(provider);
                section.Bind(obj.Configuration);
                obj.Configure();
                obj.InjectPlugin(trans);
                obj.InjectPlugin(noti);
                obj.InjectPlugin(rec);
                return obj;
            }
            else
            {
                throw new NullReferenceException($"Cannot create instance from type '{t.Name}'");
            }

        }

    }

}
