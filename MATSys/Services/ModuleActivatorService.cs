using MATSys;
using MATSys.Factories;
using MATSys.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Handler for the modules in the host
/// </summary>
public class ModuleActivatorService
{
    private readonly Dictionary<string, IConfigurationSection> _modConfigurations;
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
        _modConfigurations = provider.GetAllModuleInfos();
        _moduleFactory = provider.GetRequiredService<IModuleFactory>();
    }
    /// <summary>
    /// Create new instance using key specified in the configuration file
    /// </summary>
    /// <param name="key">Alias property in the configuration file</param>
    /// <returns>IModule instance</returns>
    public IModule GetModule(string key)
    {
        if (_modules.ContainsKey(key))
        {
            return _modules[key];
        }
        else
        {
            var mod = _moduleFactory.CreateModule(_modConfigurations[key]);
            mod.IsDisposed += ModuleDisposed;
            _modules.Add(key, mod);
            return mod;
        }
    }
    private void ModuleDisposed(object? sender, EventArgs e)
    {
        if (sender != null) _modules.Remove(((IModule)sender).Alias);
    }

}

