using MATSys.Commands;
using System.Reflection;

#if NET6_0_OR_GREATER

using System.Runtime.Loader;
using System.Runtime.Versioning;

#endif

namespace MATSys;

/// <summary>
/// Utility that dynamically load the assemblies into current appdomain
/// </summary>
public class DependencyLoader
{
    /// <summary>
    /// Load assemblies into current appdomain
    /// </summary>
    /// <param name="plugins">assembly paths</param>
    public static IEnumerable<Assembly> LoadPluginAssemblies(string[] plugins)
    {
#if NET6_0_OR_GREATER
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic).ToDictionary(x => x.Location);
        foreach (var item in plugins)
        {
            var p = Path.GetFullPath(item);
            if (File.Exists(p))
            {
                if (assemblies.Keys.Any(x => x == p))
                {
                    //if assemblies already existed, return it
                    yield return assemblies[item];
                }
                else
                {
                    var loader = new PluginLoader(p);
                    yield return loader.LoadFromAssemblyPath(p);
                }

            }

        }

#endif
#if NETFRAMEWORK
        
        var assemblyPaths = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic).Select(x => x.Location);
        foreach (var item in plugins)
        {
            var p = Path.GetFullPath(item);
            if (File.Exists(p) )
            {            
                yield return  Assembly.LoadFrom(p);
                
            }

        }
#endif
#if NETSTANDARD
        
        var assemblyPaths = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic).Select(x => x.Location);
        foreach (var item in plugins)
        {
            var p = Path.GetFullPath(item);
            if (File.Exists(p) )
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

/// <summary>
/// Exception handle class for MATSys
/// </summary>
public class ExceptionHandler
{
    /// <summary>
    /// Prefix string when command is not found
    /// </summary>
    public const string cmd_notFound = "ERR_NOTFOUND";
    /// <summary>
    /// Prefix string when command reports error during execution
    /// </summary>
    public const string cmd_execError = "ERR_EXEC";
    /// <summary>
    /// Prefix string when command reports error during serialize/deserialize process
    /// </summary>
    public const string cmd_serDesError = "ERR_SerDes";

    /// <summary>
    /// Pring the error message
    /// </summary>
    /// <param name="prefix">Prefix string</param>
    /// <param name="ex">Excption instance</param>
    /// <param name="commandString">ICommand in string format</param>
    /// <returns></returns>
    public static string PrintMessage(string prefix, Exception ex, string commandString)
    {
        return $"[{prefix}] {ex.Message} - {commandString}";
    }
    /// <summary>
    /// Pring the error message
    /// </summary>
    /// <param name="prefix">Prefix string</param>
    /// <param name="ex">Excption instance</param>
    /// <param name="command">ICommand instance</param>
    /// <returns></returns>
    public static string PrintMessage(string prefix, Exception ex, ICommand command)
    {
        return $"[{prefix}] {ex.Message} - {command.Serialize()}";
    }

}



