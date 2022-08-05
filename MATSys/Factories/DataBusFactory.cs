﻿using System.Reflection;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public sealed class DataBusFactory : IDataBusFactory
    {
        private const string prefix = "DataBus";
        private readonly Type DefaultType = typeof(EmptyDataBus);
        private readonly DependencyLoader loader;
        public IEnumerable<Type> Types { get; }
        public DataBusFactory(IConfiguration configuration)
        {
            loader = new DependencyLoader(configuration);
            Types = loader.ListTypes<IDataBus>();
        }

        public IDataBus CreateDataBus(IConfigurationSection section)
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
                    t = Types.FirstOrDefault(x => x.Name.ToLower() == $"{type}{prefix}".ToLower())!;
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

        private IDataBus CreateAndLoadInstance(Type defaultType, IConfigurationSection section)
        {
            var obj = (IDataBus)Activator.CreateInstance(defaultType)!;
            obj.Load(section);
            return obj;
        }
    }
}