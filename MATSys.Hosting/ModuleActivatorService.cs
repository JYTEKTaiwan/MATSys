using MATSys;
using MATSys.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

/// <summary>
/// Handler for the modules in the host
/// </summary>
public class ModuleActivatorService : IHostedService
{
    private readonly IModuleFactory _moduleFactory;
    private Dictionary<string, IModule> _modules = new Dictionary<string, IModule>();
    /// <summary>
    /// Count of the created modules
    /// </summary>
    public int Count => _modules.Count;

    /// <summary>
    /// Active modules in the memory
    /// </summary>
    public IModule[] ActiveModules => _modules.Values.ToArray();
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="provider">service provider from host</param>
    public ModuleActivatorService(IServiceProvider provider)
    {
        //if plugin project is reference and never used in the app project, the assembly is not list in the appdomain. 
        //we should manually load the plugin assemblies        
        var paths = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");
        //use LoadFrom to ensure it will point to the same assembly if loaded twice.
        foreach (var path in paths) Assembly.LoadFrom(path);
        _moduleFactory = provider.GetRequiredService<IModuleFactory>();
    }
    /// <summary>
    /// Create new instance using key specified in the configuration file
    /// </summary>
    /// <param name="alias">Alias property in the configuration file</param>
    /// <returns>IModule instance</returns>
    public IModule GetModule(string alias)
    {
        if (_modules.ContainsKey(alias))
        {
            return _modules[alias];
        }
        else
        {
            var mod = _moduleFactory.CreateModule(alias);
            mod.IsDisposed += ModuleDisposed;
            _modules.Add(alias, mod);
            return mod;
        }
    }
    private void ModuleDisposed(object? sender, EventArgs e)
    {
        if (sender != null) _modules.Remove(((IModule)sender).Alias);
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            foreach (var item in _modules)
            {
                item.Value.Dispose();
            }
            _modules.Clear();
        });
    }
}

