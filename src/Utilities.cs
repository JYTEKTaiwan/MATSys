using MATSys.Commands;
using NLog;
using System.Reflection;
using System.Reflection.Metadata;


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

#if NETSTANDARD      
       
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

/// <summary>
/// Type parser tool
/// </summary>
public static class TypeParser
{
    private static Logger _logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// Search the Type from entry, executing, calling  and external assemblies in sequence
    /// </summary>
    /// <param name="type">full name of the type</param>
    /// <param name="extAssemPath">external assembly path</param>
    /// <returns>Type instance</returns>
    public static Type? SearchType(string type, string extAssemPath)
    {
        if (!string.IsNullOrEmpty(type)) // return EmptyRecorder if type is empty or null
        {
            // 1.  Look up the existed assemlies in GAC
            // 1.y if existed, get the type directly and overrider the variable t
            // 1.n if not, dynamically load the assembly from the section "AssemblyPath" and search for the type

            var typeName = Assembly.CreateQualifiedName(type, type).Split(',')[0];
            if (SearchTypeInEntryAssemblies(typeName) is Type t_entry && t_entry != null)
            {
                _logger.Debug($"Found \"{t_entry.Name}\" in entry Assemblies");
                return t_entry;
            }
            else if (SearchTypeInExecutingAssemblies(typeName) is Type t_exec && t_exec != null)
            {
                _logger.Debug($"Found \"{t_exec.Name}\" in executing Assemblies");
                return t_exec;
            }
            else if (SearchTypeInCallingAssemblies(typeName) is Type t_calling && t_calling != null)
            {
                _logger.Debug($"Found \"{t_calling.Name}\" in calling Assemblies");
                return t_calling;

            }
            else
            {

                _logger.Trace($"Searching the external path \"{extAssemPath}\"");

                //load the assembly from external path
                var assem = DependencyLoader.LoadPluginAssemblies(extAssemPath).First();
                try
                {
                    var t_ext = assem.GetType(type);
                    if (t_ext != null)
                    {
                        _logger.Debug($"Found \"{t_ext.Name}\"");
                        return t_ext;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.Warn($"Exception occured when loading... \"{ex.Message}\"");
                    return null;
                }

            }
        }
        else
        {
            return null;
        }

    }
    private static Type? SearchTypeInEntryAssemblies(string typeName)
    {
        _logger.Trace($"Searching the entry assemblies");
        if (Assembly.GetEntryAssembly() is var assems && assems != null)
        {
            return assems.GetTypes().FirstOrDefault(x => x.FullName == typeName);
        }
        else
        {
            return null!;
        }

    }
    private static Type? SearchTypeInExecutingAssemblies(string typeName)
    {
        _logger.Trace($"Searching the executing assemblies");

        if (Assembly.GetExecutingAssembly() is var assems && assems != null)
        {
            return assems.GetTypes().FirstOrDefault(x => x.FullName == typeName);
        }
        else
        {
            return null!;
        }

    }
    private static Type? SearchTypeInCallingAssemblies(string typeName)
    {
        _logger.Trace($"Searching the calling assemblies");

        if (Assembly.GetCallingAssembly() is var assems && assems != null)
        {
            return assems.GetTypes().FirstOrDefault(x => x.FullName == typeName);
        }
        else
        {
            return null!;
        }

    }

}