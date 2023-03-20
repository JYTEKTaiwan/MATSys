// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Text.Json.Nodes;

Console.WriteLine("Hello, World!");


var assem = DependencyLoader.LoadPluginAssemblies(new string[] {
 "C:\\Users\\Way-Develop\\source\\repos\\MATSys\\ConsoleApp1\\bin\\Debug\\net6.0\\Recorders\\Mod1\\ClassLibrary1.dll" ,
 "C:\\Users\\Way-Develop\\source\\repos\\MATSys\\ConsoleApp1\\bin\\Debug\\net6.0\\Recorders\\Mod2\\ClassLibrary1.dll" }).ToArray();

var t = assem[0].GetType("ClassLibrary1.Class1");
var obj = Activator.CreateInstance(t);
obj.GetType().GetMethod("DoCSV").Invoke(obj,new object[] { });

var t2 = assem[1].GetType("ClassLibrary1.Class1");
var obj2 = Activator.CreateInstance(t2);
obj2.GetType().GetMethod("DoCSV").Invoke(obj2, new object[] { });


Console.ReadLine();


public class DependencyLoader
{
    /// <summary>
    /// Load assemblies into current appdomain
    /// </summary>
    /// <param name="plugins">assembly paths</param>
    public static IEnumerable<Assembly> LoadPluginAssemblies(string[] plugins)
    {
#if NET6_0_OR_GREATER
        var assemblyPaths = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic).Select(x => x.Location);
        foreach (var item in plugins)
        {
            var p = Path.GetFullPath(item);
            if (File.Exists(p) && !assemblyPaths.Any(x => x == p))
            {

                var loader = new PluginLoader(p);
                yield return loader.LoadFromAssemblyPath(p);

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


public class ParamType
{
    public string A { get; set; }
    public int B { get; set; }
}