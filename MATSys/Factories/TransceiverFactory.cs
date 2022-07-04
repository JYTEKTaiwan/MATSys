﻿using System.Reflection;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public sealed class TransceiverFactory : ITransceiverFactory
    {
        private const string prefix = "Transceiver";
        private readonly Type DefaultType = typeof(EmptyTransceiver);        
        public  readonly IEnumerable<Type> _types;
        public TransceiverFactory(DependencyLoader loader)
        {
           
            _types = loader.ListModuleTypes<ITransceiver>();
        }

        public ITransceiver CreateTransceiver(IConfigurationSection section)
        {
            try
            {
                //check if section in the json configuration exits
                if (section.Exists())
                {

                    Type t;

                    string type = section.GetValue<string>("Type");
                    if (string.IsNullOrEmpty(type))
                    {
                        //if key is empty or null, return immediately with dafault datarecorder object.
                        return CreateAndLoadInstance(DefaultType, section);
                    }

                    //if key has value, search the type with the default class name. eg. xxx=>xxxDataRecorder
                    t = _types.FirstOrDefault(x => x.Name.ToLower() == $"{type}{prefix}".ToLower())!;
                    if (t == null)
                    {
                        //cannot parse any type, use default datarecorder
                        return CreateAndLoadInstance(DefaultType, section);
                    }
                    else
                    {
                        //dynamically load the assembly and object                    
                        return CreateAndLoadInstance(t, section);
                    }
                }
                else
                {
                    //section is missing, use default DataRecorder object.
                    return CreateAndLoadInstance(DefaultType, section);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private ITransceiver CreateAndLoadInstance(Type defaultType, IConfigurationSection section)
        {
            var obj = (ITransceiver)Activator.CreateInstance(defaultType)!;
            obj.Load(section);
            return obj;
        }

    }

}