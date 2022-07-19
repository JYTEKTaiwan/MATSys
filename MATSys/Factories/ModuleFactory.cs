using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MATSys.Factories
{
    public sealed class ModuleFactory : IModuleFactory
    {
        private const string sectionKey = "Plugins:Devices";
        private readonly IServiceProvider _services;
        public List<DeviceInformation> DeviceInfos { get; }

        public ModuleFactory(IServiceProvider services)
        {
            _services = services;
            var config = _services.GetRequiredService<IConfiguration>();
            var plugins = config.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();

            //Load plugin assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);

            DeviceInfos = ListDevices(config);
        }

        public IModule CreateDevice(DeviceInformation info)
        {
            return (IModule)Activator.CreateInstance(info.DeviceType!, new object[] { _services, info.Name })!;
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