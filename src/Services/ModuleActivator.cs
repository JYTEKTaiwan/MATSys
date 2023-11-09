using MATSys;
using MATSys.Factories;

/// <summary>
/// Handler for the modules in the host
/// </summary>
public class ModuleActivator
{
    private readonly IServiceProvider _provider;
    private readonly Dictionary<string, IConfigurationSection> _modConfigurations;
    private readonly IModuleFactory _moduleFactory;
    private Dictionary<string, IModule> _modules = new Dictionary<string, IModule>();
    /// <summary>
    /// Count of the created modules
    /// </summary>
    public int Count => _modules.Count;
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="provider">service provider from host</param>
    public ModuleActivator(IServiceProvider provider)
    {
        _provider = provider;
        var modSection=provider.GetRequiredService<IConfiguration>().GetSection("MATSys:Modules");
        if (!modSection.Exists()) throw new ArgumentNullException("MATSys:Modules seciton is not existed in the configuration file");
        _modConfigurations = modSection.GetChildren().ToDictionary(x => x["Alias"]!);
        _moduleFactory = _provider.GetRequiredService<IModuleFactory>();
    }
    /// <summary>
    /// Create new instance using key specified in the configuration file
    /// </summary>
    /// <param name="key">Alias property in the configuration file</param>
    /// <returns>IModule instance</returns>
    public IModule Create(string key)
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