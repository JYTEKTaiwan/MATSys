using System.Reflection;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public sealed class DataBusFactory : IDataBusFactory
    {
        public DataBusFactory(IConfiguration configuration)
        {
            DataBusContext.Configure(configuration);
            //Register assembly resolve event(in case that dynamically loaded assembly had dependent issue)
            AppDomain.CurrentDomain.AssemblyResolve += DataBusContext.Instance.AssemblyResolve;
        }

        public IDataBus CreatePublisher(IConfigurationSection section)
        {
            return DataBusContext.Instance.CreateInstance(section);
        }
    }

    internal sealed class DataBusContext
    {
        private const string TypePrefix = "DataBus";
        private const string SectionKey = "DataBusFactory";
        private Type DefaultType { get; } = typeof(EmptyDataBus);
        public List<Type> Types { get; } = new List<Type>();
        public IConfiguration? Configuration { get; }
        public string ModulesFolder { get; } = @".\modules\";
        public string LibrariesFolder { get; } = @".\libs\";

        private static Lazy<DataBusContext> lazy = new Lazy<DataBusContext>();

        public static DataBusContext Instance => lazy.Value;

        public static void Configure(IConfiguration config)
        {
            if (!lazy.IsValueCreated)
            {
                lazy = new Lazy<DataBusContext>(() => new DataBusContext(config));
            }
        }

        private DataBusContext(IConfiguration config)
        {
            Configuration = config;


            string baseFolder = AppDomain.CurrentDomain.BaseDirectory;

            //load library folder path from configuration, if not,  use .\libs\
            var temp = config.GetValue<string>("LibrariesFolder");
            LibrariesFolder = string.IsNullOrEmpty(temp) ? Path.Combine(baseFolder, "libs") : temp;

            //loadmodules folder path from configuration, if not,  use .\modules\
            var tempRoot = config.GetValue<string>("ModulesFolder");
            ModulesFolder = string.IsNullOrEmpty(tempRoot) ? Path.Combine(baseFolder, "modules") : tempRoot;

            //Find all assemblies inherited from IDataRecorder
            foreach (var item in Directory.GetFiles(ModulesFolder, "*.dll"))
            {
                var p = Path.GetFullPath(item);
                var types = Assembly.LoadFile(p).GetTypes().Where
                    (x => x.GetInterface(typeof(IDataBus).FullName!) != null);
                Types.AddRange(types);
            }

        }
        public Assembly? AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            string s = LibrariesFolder + args.Name.Remove(args.Name.IndexOf(',')) + ".dll";
            if (File.Exists(s))
            {
                return Assembly.LoadFile(Path.GetFullPath(s));
            }
            else
            {
                throw new FileNotFoundException($"Dependent assembly not found : {args.Name}");
            }
        }
        public IDataBus CreateInstance(IConfigurationSection section)
        {
            IDataBus obj;

            //check if section in the json configuration exits
            if (section.Exists())
            {
                var customNameLUT = Configuration?.GetSection(SectionKey);

                Type t;

                string type = section.GetValue<string>("Type");
                if (string.IsNullOrEmpty(type))
                {
                    //if key is empty or null, return immediately with dafault datarecorder object.
                    return CreateAndLoadInstance(DefaultType, section);
                }

                //if key has value, search the type with the default class name. eg. xxx=>xxxDataRecorder
                t = Types.Find(x => x.Name.ToLower() == $"{type}{TypePrefix}".ToLower())!;
                if (t == null)
                {
                    //if searching failed, use the lookup table to get the custom name
                    var key = customNameLUT.GetValue<string>(type.ToLower());
                    if (string.IsNullOrEmpty(key))
                    {
                        return CreateAndLoadInstance(DefaultType, section);
                    }
                    else
                    {
                        t = Types.Find(x => x.Name.ToLower() == key)!;
                    }
                }

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
        private IDataBus CreateAndLoadInstance(Type defaultType, IConfigurationSection section)
        {
            var obj = (IDataBus)Activator.CreateInstance(defaultType)!;
            obj.Load(section);
            return obj;
        }
    }

}