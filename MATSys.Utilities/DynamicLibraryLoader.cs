using System.Reflection;

#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif

namespace MATSys.Utilities;

/// <summary>
/// Load the assembly dynamically from external file
/// </summary>
internal class DynamicLibraryLoader
{
    /// <summary>
    /// Load assemblies into current appdomain
    /// </summary>
    /// <param name="plugins">assembly paths</param>
    public static IEnumerable<Assembly> LoadPluginAssemblies(params string[] plugins)
    {
        //check parameter
        if (plugins == null)
        {
            throw new ArgumentNullException("parameter is null");
        }
        if (plugins.Any(x => !File.Exists(x)))
        {
            throw new ArgumentException("path is not existed");
        }

#if NET6_0_OR_GREATER
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic).ToList();
        foreach (var item in plugins)
        {
            var p = Path.GetFullPath(item);
            if (File.Exists(p))
            {
                if (assemblies.Any(x => x.Location == p))
                {
                    //if assemblies already existed, return it
                    yield return assemblies.First(x => x.Location == p);
                }
                else
                {
                    var loader = new PluginLoader(p);
                    yield return loader.LoadFromAssemblyPath(p);
                }

            }

        }

#else 

        foreach (var item in plugins)
        {
            var p = Path.GetFullPath(item);
            if (File.Exists(p))
            {
                yield return Assembly.LoadFrom(p);
            }

        }
#endif
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Plugin class for .net6
    /// </summary>
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

}

