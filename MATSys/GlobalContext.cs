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

        var modTemp = config.GetValue<string>("ModulesFolder");
        ModulesFolder = string.IsNullOrEmpty(modTemp) ? DefaultPathInfo.ModulesFolder : modTemp;
        var libTemp = config.GetValue<string>("LibrariesFolder");
        LibrariesFolder = string.IsNullOrEmpty(libTemp) ? DefaultPathInfo.LibraryFolder : libTemp;

    }
    public DependencyLoader(string modFolder, string libFolder)
    {
        ModulesFolder = string.IsNullOrEmpty(modFolder) ? DefaultPathInfo.ModulesFolder : modFolder;
        LibrariesFolder = string.IsNullOrEmpty(libFolder) ? DefaultPathInfo.LibraryFolder : libFolder;

    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        throw (Exception)e.ExceptionObject;
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

    public IEnumerable<Type> ListModuleTypes<T>() where T : IModule
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
    public IEnumerable<DeviceInformation> ListDevices(IConfiguration config)
    {
        if (config.GetSection("Devices").Exists())
        {          

            var pairs = config.GetSection("Devices").AsEnumerable(true).Where(x => x.Value != null);
            foreach (var item in pairs)
            {
                var info = Parse(item);
                if (!string.IsNullOrEmpty(info.Name))
                {
                    yield return info;
                }
            }

        }
    }

    private DeviceInformation Parse(KeyValuePair<string, string> kvPair)
    {
        var searchKey = kvPair.Key.Contains(':') ? kvPair.Key.Split(':')[0] : kvPair.Key;

        var info = DeviceInformation.Empty;

        bool isModFound = false;
        foreach (var item in Directory.GetFiles(Path.GetFullPath(ModulesFolder), "*.dll"))
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


internal class DefaultPathInfo
{
    public static string ModulesFolder { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules");
    public static string LibraryFolder { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libs");

}