using System.Reflection;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public sealed class DataRecorderFactory : IDataRecorderFactory
    {
        public DataRecorderFactory(IConfiguration configuration)
        {
            DataRecorderContext.Configure(configuration);
            //Register assembly resolve event(in case that dynamically loaded assembly had dependent issue)
            AppDomain.CurrentDomain.AssemblyResolve += DataRecorderContext.Instance.AssemblyResolve;
       
        }

        public IDataRecorder CreateRecorder(IConfigurationSection section)
        {
            return DataRecorderContext.Instance.CreateInstance(section);
        }


    }
    internal sealed class DataRecorderContext
    {
        private const string TypePrefix = "DataRecorder";
        private const string SectionKey = "DataRecorderFactory";
        private Type DefaultType { get; } = typeof(EmptyDataRecorder);
        public List<Type> Types { get; } = new List<Type>();
        public IConfiguration? Configuration { get; }
        public string ModulesFolder { get; } = @".\modules\";
        public string LibrariesFolder { get; } = @".\libs\";

        private static Lazy<DataRecorderContext> lazy = new Lazy<DataRecorderContext>();

        public static DataRecorderContext Instance => lazy.Value;

        public static void Configure(IConfiguration config)
        {
            if (!lazy.IsValueCreated)
            {
                lazy = new Lazy<DataRecorderContext>(() => new DataRecorderContext(config));
            }
        }

        private DataRecorderContext(IConfiguration config)
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
            foreach (var item in Directory.GetFiles(Path.GetFullPath(ModulesFolder), "*.dll"))
            {
                var assem = Assembly.LoadFile(item);
                //load dependent assemblies
                foreach (var dependent in assem.GetReferencedAssemblies())
                {
                    Assembly.Load(dependent);
                }
                var types = assem.GetTypes().Where
                    (x => x.GetInterface(typeof(IDataRecorder).FullName!) != null);
                Types.AddRange(types);
            }

        }
        public Assembly? AssemblyResolve(object? sender, ResolveEventArgs args)
        {            
            string s1 = args.Name.Remove(args.Name.IndexOf(',')) + ".dll";
            string s2 = LibrariesFolder + args.Name.Remove(args.Name.IndexOf(',')) + ".dll";
            if(File.Exists(s1))
            {
                return Assembly.LoadFile(Path.GetFullPath(s1));
            }
            else if (File.Exists(s2))
            {
                return Assembly.LoadFile(Path.GetFullPath(s2));
            }
            else
            {
                throw new FileNotFoundException($"Dependent assemaaably not found : {args.Name}");
            }
        }
        public IDataRecorder CreateInstance(IConfigurationSection section)
        {
            IDataRecorder obj;

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
        private IDataRecorder CreateAndLoadInstance(Type defaultType, IConfigurationSection section)
        {
            var obj = (IDataRecorder)Activator.CreateInstance(defaultType)!;
            obj.Load(section);
            return obj;
        }
    }
}