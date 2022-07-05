using MATSys.Plugins;
using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public sealed class TransceiverFactory : ITransceiverFactory
    {
        private const string sectionKey = "Plugins:Transceivers";
        private const string prefix = "Transceiver";
        private readonly Type DefaultType = typeof(EmptyTransceiver);
        private readonly IConfiguration _configuration;

        public TransceiverFactory(IConfiguration config)
        {
            _configuration = config;
            var plugins = _configuration.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();

            //Load plugin assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);
        }

        public ITransceiver CreateTransceiver(IConfigurationSection section)
        {
            try
            {
                Type t = DefaultType;
                //check if section in the json configuration exits
                if (section.Exists())
                {
                    var assems = AppDomain.CurrentDomain.GetAssemblies();
                    string type = section.GetValue<string>("Type");
                    if (!string.IsNullOrEmpty(type))
                    {
                        //if key has value, search the type with the default class name. eg. xxx=>xxxDataRecorder
                        foreach (var assem in assems)
                        {
                            var dummy = assem.GetTypes().FirstOrDefault(x => x.Name.ToLower() == $"{type}{prefix}".ToLower());
                            if (dummy == null)
                            {
                                continue;
                            }
                            else
                            {
                                t = dummy;
                                break;
                            }
                        }
                    }
                }
                return CreateAndLoadInstance(t, section);
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