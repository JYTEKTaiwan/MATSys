using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MATSys.Factories
{
    public sealed class ModuleFactory : IModuleFactory
    {
        private const string sectionKey = "Plugins:Devices";
        private const string key_recorder = "Recorder";
        private const string key_notifier = "Notifier";
        private const string key_transceiver = "Transceiver";

        private readonly IServiceProvider _services;

        private readonly IConfiguration _config;
        private readonly ITransceiverFactory _transceiverFactory;
        private readonly INotifierFactory _notifierFactory;
        private readonly IRecorderFactory _recorderFactory;
        public List<DeviceInformation> DeviceInfos { get; }
        public ModuleFactory(IConfiguration config, ITransceiverFactory tran_factory, INotifierFactory noti_factory, IRecorderFactory rec_factory)
        {
            _config = config;
            _transceiverFactory = tran_factory;
            _notifierFactory = noti_factory;
            _recorderFactory = rec_factory;
            var plugins = config.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();
            //Load plugin assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);

            DeviceInfos = ListDevices(config);
        }

        public IModule CreateDevice(DeviceInformation info)
        {
            var section = _config.GetSection(info.Name);
            var trans = _transceiverFactory.CreateTransceiver(section.GetSection(key_transceiver));
            var noti = _notifierFactory.CreateNotifier(section.GetSection(key_notifier));
            var rec = _recorderFactory.CreateRecorder(section.GetSection(key_recorder));
            return (IModule)Activator.CreateInstance(info.DeviceType!, new object[] { section, trans,noti,rec,info.Name })!;
        }

        public List<DeviceInformation> ListDevices(IConfiguration? config = null)
        {
            var devices = new List<DeviceInformation>();
            if (config != null && config!.GetSection("Devices").Exists())
            {
                //List all available assemblies
                var assems = AppDomain.CurrentDomain.GetAssemblies();

                //Parse all assigned devices from configuration file
                var pairs = config.GetSection("Devices").AsEnumerable(true).Where(x => x.Value != null);

                foreach (var item in pairs)
                {
                    var searchKey = item.Key.Contains(':') ? item.Key.Split(':')[0] : item.Key;
                    //search the type by name in the assemblies
                    foreach (var assem in assems)
                    {
                        var t = assem.GetType(searchKey);
                        if (t != null)
                        {
                            devices.Add(new DeviceInformation(t, item.Value));
                            break;
                        }
                    }
                }
            }
            return devices;
        }

        public static T CreateNew<T>(object parameter, ITransceiver transceiver, INotifier notifier, IRecorder recorder) where T:IModule
        {
            return (T)Activator.CreateInstance(typeof(T),new object[] {parameter,transceiver,notifier,recorder});
        }
        public static IModule CreateNew(Type moduleType,object parameter, ITransceiver transceiver, INotifier notifier, IRecorder recorder)
        {
            if (typeof(IModule).IsAssignableFrom(moduleType))
            {
                return (IModule)Activator.CreateInstance(moduleType, new object[] { parameter, transceiver, notifier, recorder });
            }
            else
            {
                return null;
            }
        }
    }
}