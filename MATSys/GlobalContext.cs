using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace MATSys;


public class DependencyLoader
{
    public string ModulesFolder { get; }
    public string LibrariesFolder { get; }

    public DependencyLoader(IConfiguration config)
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        var template = new DependencyInformation();

        var modTemp =config.GetValue<string>("ModulesFolder");
        ModulesFolder=string.IsNullOrEmpty(modTemp)?template.ModulesFolder:modTemp;
        var libTemp = config.GetValue<string>("LibrariesFolder");
        LibrariesFolder = string.IsNullOrEmpty(libTemp) ? template.LibraryFolder : libTemp;

    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        throw e.ExceptionObject as Exception;
    }

    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        string s1 = args.Name.Remove(args.Name.IndexOf(',')) + ".dll";
        string s2 = Path.Combine(LibrariesFolder, args.Name.Remove(args.Name.IndexOf(',')) + ".dll");
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
            throw new FileLoadException($"Assembly not found {args.Name}");
        }
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