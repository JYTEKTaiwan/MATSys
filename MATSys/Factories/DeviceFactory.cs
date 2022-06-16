using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MATSys.Factories
{
    public sealed class DeviceFactory : IDeviceFactory
    {
        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        public List<DeviceInformation> DeviceInfos { get; } = new List<DeviceInformation>();

        public DeviceFactory(IServiceProvider services)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            _services = services;
            _config = services.GetRequiredService<IConfiguration>();
            DeviceFactoryContext.Configure(_config);
            DeviceInfos = DeviceFactoryContext.Instance.DeviceInfos;

        }

        private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            string baseFolder = AppDomain.CurrentDomain.BaseDirectory;
            string s1 = args.Name.Remove(args.Name.IndexOf(',')) + ".dll";
            string s2 = Path.Combine(baseFolder,"libs",args.Name.Remove(args.Name.IndexOf(',')) + ".dll") ;
            if (File.Exists(s1))
            {
                return Assembly.LoadFile(Path.GetFullPath(s1));
            }
            else if (File.Exists(s2))
            {
                return Assembly.LoadFile(Path.GetFullPath(s2));
            }
            else
            {
                throw new FileNotFoundException($"Dependent assembly not found : {args.Name}");
            }
        }

        public IDevice CreateDevice(DeviceInformation info)
        {
            return (IDevice)Activator.CreateInstance(info.DeviceType!, new object[] { _services, info.Name })!;
        }
    }

    public struct DeviceInformation
    {
        public Type DeviceType { get; }
        public string Name { get; }

        public DeviceInformation(Type deviceType, string name)
        {
            DeviceType = deviceType;
            Name = name;
        }

        public static DeviceInformation Empty => new DeviceInformation(null!, "");
    }

    internal sealed class DeviceFactoryContext
    {
        private const string TypePrefix = "DataBus";
        private const string SectionKey = "DataBusFactory";
        public string ModulesFolder { get; } = @".\modules\";
        public string LibrariesFolder { get; } = @".\libs\";
        public List<DeviceInformation> DeviceInfos { get; } = new List<DeviceInformation>();
        private static Lazy<DeviceFactoryContext> lazy = new Lazy<DeviceFactoryContext>();

        public static DeviceFactoryContext Instance => lazy.Value;

        public static void Configure(IConfiguration config)
        {
            if (!lazy.IsValueCreated)
            {
                lazy = new Lazy<DeviceFactoryContext>(() => new DeviceFactoryContext(config));
            }
        }

        private DeviceFactoryContext(IConfiguration config)
        {
            if (config.GetSection("Devices").Exists())
            {
                var pairs = config.GetSection("Devices").AsEnumerable(true).Where(x => x.Value != null);
                foreach (var item in pairs)
                {
                    var info = Parse(config, item);
                    if (!string.IsNullOrEmpty(info.Name))
                    {
                        DeviceInfos.Add(info);
                    }
                }
            }
        }
        private DeviceInformation Parse(IConfiguration configuration, KeyValuePair<string, string> kvPair)
        {
            var searchKey = kvPair.Key.Contains(':') ? kvPair.Key.Split(':')[0] : kvPair.Key;

            var info = DeviceInformation.Empty;

            string libFolder, modFolder = "";
            string baseFolder = AppDomain.CurrentDomain.BaseDirectory;

            //load library folder path from configuration, if not,  use .\libs\
            var temp = configuration.GetValue<string>("LibrariesFolder");
            libFolder = string.IsNullOrEmpty(temp) ? Path.Combine(baseFolder, @"libs") : temp;

            //load modules folder path from configuration, if not,  use .\modules\
            var tempRoot = configuration.GetValue<string>("ModulesFolder");
            modFolder = string.IsNullOrEmpty(tempRoot) ? Path.Combine(baseFolder, @"modules") : tempRoot;

            bool isModFound = false;
            foreach (var item in Directory.GetFiles(Path.GetFullPath(modFolder), "*.dll"))
            {

                var types = Assembly.LoadFile(item).GetTypes().Where
                    (x => x.Name == searchKey && x.GetInterface(typeof(IDevice).FullName!) != null);
                if (types.Count() > 0)
                {
                    info = new DeviceInformation(types.First(), kvPair.Value);
                    isModFound = true;
                    break;
                }
                else
                {
                    continue;
                }
            }

            if (!isModFound)
            {
                foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var types = item.GetTypes().Where
                        (x => x.Name == searchKey && x.GetInterface(typeof(IDevice).FullName!) != null);
                    if (types.Count() > 0)
                    {
                        info = new DeviceInformation(types.First(), kvPair.Value);
                        isModFound = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return isModFound ? info : DeviceInformation.Empty;
        }

    }

}