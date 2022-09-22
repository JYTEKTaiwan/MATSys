using MATSys.Commands;
using System.Linq;
using System.Reflection;

#if NET6_0_OR_GREATER

using System.Runtime.Loader;

#endif

namespace MATSys;

public class DependencyLoader
{
    public static void LoadPluginAssemblies(string[] plugins)
    {
        var assemblyPaths = AppDomain.CurrentDomain.GetAssemblies().Select(x=>x.Location);
        foreach (var item in plugins)
        {
            var p = Path.GetFullPath(item);
            if (File.Exists(p) && !assemblyPaths.Any(x=>x==p))
            {
#if NET6_0_OR_GREATER
                var loader = new PluginLoader(p);
                var assem=loader.LoadFromAssemblyPath(p);               
#endif
#if NETSTANDARD2_0_OR_GREATER
 var assem=Assembly.LoadFile(p);
#endif
            }

        }
    }
}

public class MATSysHelper
{

    public static IEnumerable<MATSysIServiceContext> ListAllPlugins()
    {
        string subFolder = @".\modules";
        if (Directory.Exists(subFolder))
        {
            DependencyLoader.LoadPluginAssemblies(Directory.GetFiles(subFolder));
        }

        Type t = typeof(IPlugin);

        //search executing assembly
        foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var item in assem.GetTypes())
            {
                if (t.IsAssignableFrom(item) && !item.IsAbstract && !item.IsInterface)
                {
                    yield return new MATSysIServiceContext()
                    {
                        Type = Parse(item.Name).name,
                        Category = Parse(item.Name).type,
                        Alias = item.Name,
                        AssemblyPath = assem.Location
                    };
                }
            }
        }
    }

    public static IEnumerable<MATSysIServiceContext> ListAllModules()
    {
        string subFolder = @".\modules";
        if (Directory.Exists(subFolder))
        {
            DependencyLoader.LoadPluginAssemblies(Directory.GetFiles(subFolder));
        }


        Type t = typeof(IModule);

        //search executing assembly
        foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var item in assem.GetTypes())
            {
                if (t.IsAssignableFrom(item) && !item.IsAbstract && !item.IsInterface)
                {
                    yield return new MATSysIServiceContext()
                    {
                        Alias = item.Name,
                        Type = item.Name,
                        Category = "Modules",
                        AssemblyPath = assem.Location
                    };

                }

            }
        }

    }

    private static (string name, string type) Parse(string input)
    {

        var name = "";
        var type = "";
        var option = StringSplitOptions.None;
        if (input.Contains("Transceiver"))
        {
            name = input.Split(new string[] { "Transceiver"}, option)[0];
            type = "Transceiver";
        }
        else if (input.Contains("Notifier"))
        {
            name = input.Split(new string[] { "Notifier" }, option)[0];
            type = "Notifier";
        }
        else if (input.Contains("Recorder"))
        {
            name = input.Split(new string[] { "Recorder" }, option)[0];
            type = "Recorder";
        }
        else
        {
            name = "";
            type = "";
        }
        name = name.ToLower();
        name = name == "empty" ? "" : name;
        return (name, type);



    }

}

public class ModuleHelper
{
    public static Type? GetModuleType(string typeName)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
        return types.FirstOrDefault(x => x.Name == typeName);
    }
    public static IEnumerable<string> ShowSupportedCommands(string typeName)
    {
        var t = GetModuleType(typeName);
        return ShowSupportedCommands(t);
    }
    public static IEnumerable<string> ShowSupportedCommands<T>() where T : IModule
    {
        return ShowSupportedCommands(typeof(T));
    }
    public static IEnumerable<string> ShowSupportedCommands(Type t) 
    {
        if (typeof(IModule).IsAssignableFrom(t))
        {
            var methods = t.GetMethods().Where(x => x.GetCustomAttribute<MATSysCommandAttribute>() != null);
            foreach (var mi in methods)
            {
                var cmd = mi.GetCustomAttribute<MATSysCommandAttribute>();
                cmd.ConfigureCommandType(mi);
                yield return cmd.GetTemplateString();
            }
        }
        

    }
}

public class MATSysIServiceContext
{
    public string Alias { get; set; }
    public string Category { get; set; }
    public string AssemblyPath { get; set; }
    public string Type { get; set; }

}

#if NET6_0_OR_GREATER
/// <summary>
/// Plugin class for .net6
/// </summary>
internal class PluginLoader : AssemblyLoadContext
{
    private AssemblyDependencyResolver resolver;

    public PluginLoader(string? name, bool isCollectible = true) : base(name, isCollectible)
    {
        resolver = new AssemblyDependencyResolver(name!);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string assemblyPath = resolver.ResolveAssemblyToPath(assemblyName)!;
        if (assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }
        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName)!;
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
}
#endif

internal class ExceptionHandler
{
    public static string PrintMessage(string prefix, Exception ex, string commandString)
    {
        return $"[{prefix}] {ex.Message} - {commandString}";
    }
    public static string PrintMessage(string prefix, Exception ex, ICommand command)
    {
        return $"[{prefix}] {ex.Message} - {command.Serialize()}";
    }

}

