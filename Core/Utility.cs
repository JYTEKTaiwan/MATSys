using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MATSys
{
    internal sealed class Utility
    {
        internal sealed class ModuleContext
        {
            public List<Type> Types { get; set; } = new List<Type>();
            public string SectionKey { get; set; } = "";
            public string TypePrefix { get; set; } = "";
            public IConfiguration? Configuration { get; set; }
            public string ModulesFolder { get; set; } = @".\modules\";
            public string LibrariesFolder { get; set; } = @".\libs\";

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

            internal static ModuleContext Parse<TInterface>(IConfiguration configuration, string key) where TInterface : IModule
            {
                var context = new ModuleContext();
                context.SectionKey = key;
                context.Configuration = configuration;
                context.TypePrefix = typeof(TInterface).Name.Remove(0, 1);

                //load library folder path from configuration, if not,  use .\libs\
                var temp = configuration.GetValue<string>("LibrariesFolder");
                context.LibrariesFolder = string.IsNullOrEmpty(temp) ? @".\libs\" : temp;

                //loadmodules folder path from configuration, if not,  use .\modules\
                var tempRoot = configuration.GetValue<string>("ModulesFolder");
                context.ModulesFolder = string.IsNullOrEmpty(tempRoot) ? @".\modules\" : tempRoot;

                //Find all assemblies inherited from IDataRecorder
                foreach (var item in Directory.GetFiles(context.ModulesFolder, "*.dll"))
                {
                    var p = Path.GetFullPath(item);
                    var types = Assembly.LoadFile(p).GetTypes().Where
                        (x => x.GetInterface(typeof(TInterface).FullName!) != null);
                    context.Types.AddRange(types);
                }

                return context;
            }

            public TInterface CreateInstance<TInterface>(IConfigurationSection section, Type defaultInstace) where TInterface : IModule
            {
                TInterface obj;

                //check if section in the json configuration exits
                if (section.Exists())
                {
                    var lookup = Configuration?.GetSection(SectionKey);

                    Type t;

                    string type = section.GetValue<string>("Type");
                    if (string.IsNullOrEmpty(type))
                    {
                        //if key is empty or null, return immediately with dafault datarecorder object.
                        obj = (TInterface)Activator.CreateInstance(defaultInstace)!;
                        obj.Load(section);
                        return obj;
                    }

                    //if key has value, search the type with the default class name. eg. xxx=>xxxDataRecorder
                    t = Types.Find(x => x.Name.ToLower() == $"{type}{TypePrefix}".ToLower())!;
                    if (t == null)
                    {
                        //if searching failed, use the lookup table to get the custom name
                        var key = lookup.GetValue<string>(type.ToLower());
                        if (string.IsNullOrEmpty(key))
                        {
                            obj = (TInterface)Activator.CreateInstance(defaultInstace)!;
                            obj.Load(section);
                            return obj;
                        }
                        else
                        {
                            t = Types.Find(x => x.Name.ToLower() == key)!;
                        }
                    }

                    if (t == null)
                    {
                        //cannot parse any type, use default datarecorder
                        obj = (TInterface)Activator.CreateInstance(defaultInstace)!;
                        obj.Load(section);
                        return obj;
                    }
                    else
                    {
                        //dynamically load the assembly and object
                        obj = (TInterface)Activator.CreateInstance(t)!;
                        obj.Load(section);
                        return obj;
                    }
                }
                else
                {
                    //section is missing, use default DataRecorder object.
                    obj = (TInterface)Activator.CreateInstance(defaultInstace)!;
                    obj.Load(section);
                    return obj;
                }
            }
        }
    }
}