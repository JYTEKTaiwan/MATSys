using System.Reflection;

#if NET6_0_OR_GREATER

using System.Runtime.Loader;

#endif

namespace MATSys;

public class DependencyLoader
{
    public static void LoadPluginAssemblies(string[] plugins)
    {
        foreach (var item in plugins)
        {
            var p = Path.GetFullPath(item);
            if (File.Exists(p))
            {
#if NET6_0_OR_GREATER
                var loader = new PluginLoader(p);
                loader.LoadFromAssemblyPath(p);

#endif
#if NETSTANDARD2_0_OR_GREATER
 Assembly.LoadFile(p);
#endif
            }

        }
    }
}

#if NET6_0_OR_GREATER

internal class PluginLoader : AssemblyLoadContext
{
    private AssemblyDependencyResolver resolver;

    public PluginLoader(string? name, bool isCollectible = false) : base(name, isCollectible)
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
