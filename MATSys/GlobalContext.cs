using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace MATSys;


internal class DependencyLoader
{
    private static Lazy<DependencyLoader> _lazy = new Lazy<DependencyLoader>(() => new DependencyLoader());
    public string ModulesFolder { get; }
    public string LibrariesFolder { get; }

    public static DependencyLoader Instance => _lazy.Value;
    private DependencyLoader()
    {
        var template = new DependencyInformation();

        var path = "appsettings.json";
        if (File.Exists(path))
        {
            var str = File.ReadAllText(path);
            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<DependencyInformation>(str);
            //load library folder path from configuration, if not,  use .\libs\        
            if (config != null)
            {
                LibrariesFolder = config.LibraryFolder;
                ModulesFolder = config.ModulesFolder;
            }
            else
            {

                LibrariesFolder = template.LibraryFolder;
                ModulesFolder = template.ModulesFolder;
            }

        }
        else
        {
            LibrariesFolder = template.LibraryFolder;
            ModulesFolder = template.ModulesFolder;
        }


    }
    public DependencyLoader(IConfiguration config)
    {
        var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
        //load library folder path from configuration, if not,  use .\libs\
        var temp = config.GetValue<string>("LibrariesFolder");
        LibrariesFolder = string.IsNullOrEmpty(temp) ? Path.Combine(baseFolder, "libs") : temp;

        //loadmodules folder path from configuration, if not,  use .\modules\
        var tempRoot = config.GetValue<string>("ModulesFolder");
        ModulesFolder = string.IsNullOrEmpty(tempRoot) ? Path.Combine(baseFolder, "modules") : tempRoot;
    }
    public IEnumerable<Type> ListTypes<T>() where T : IModule
    {
        //Find all assemblies inherited from IDataRecorder
        foreach (var item in Directory.GetFiles(Path.GetFullPath(ModulesFolder), "*.dll"))
        {
            var assem = Assembly.LoadFile(item);
            //load dependent assemblies        
            var types = assem.GetTypes().Where
                (x => x.GetInterface(typeof(T).FullName!) != null);
            foreach (var t in types)
            {
                yield return t;
            }
        }
    }
    public IEnumerable<Type> ListTypes<T>(Func<Type, bool> func) where T : IModule
    {
        //Find all assemblies inherited from IDataRecorder
        foreach (var item in Directory.GetFiles(Path.GetFullPath(ModulesFolder), "*.dll"))
        {
            var assem = Assembly.LoadFile(item);
            //load dependent assemblies        
            var types = assem.GetTypes().Where(func);
            foreach (var t in types)
            {
                yield return t;
            }
        }
    }
    private TModule CreateAndLoadInstance<TModule>(Type defaultType, IConfigurationSection section) where TModule : IModule
    {
        var obj = (TModule)Activator.CreateInstance(defaultType)!;
        obj.Load(section);
        return obj;
    }



}



internal class DependencyInformation
{
    public string ModulesFolder { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules");
    public string LibraryFolder { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libs");

}