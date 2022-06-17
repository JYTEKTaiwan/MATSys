using System.Reflection;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public sealed class DataRecorderFactory : IDataRecorderFactory
    {

        private const string prefix = "DataRecorder";
        private readonly Type DefaultType = typeof(EmptyDataRecorder);
        private readonly DependencyLoader loader;
        public IEnumerable<Type> Types { get; }
        public DataRecorderFactory(IConfiguration configuration)
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
            Types = DependencyLoader.Instance.ListTypes<IDataRecorder>();
        }

        public IDataRecorder CreateRecorder(IConfigurationSection section)
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
        private Assembly? AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            string s1 = args.Name.Remove(args.Name.IndexOf(',')) + ".dll";
            string s2 = Path.Combine(loader.LibrariesFolder, args.Name.Remove(args.Name.IndexOf(',')) + ".dll");
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
                throw new FileLoadException($"Dependent assembly not found : {args.Name}");
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