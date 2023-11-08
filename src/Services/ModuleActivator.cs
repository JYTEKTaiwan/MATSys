

using System.Runtime.InteropServices;
using MATSys;
using MATSys.Factories;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ModuleActivator
{
    private readonly IServiceProvider _provider;
    private readonly Dictionary<string,IConfigurationSection> _modConfigurations;
    private readonly IModuleFactory _moduleFactory;
    private  Dictionary<string, IModule> _modules=new Dictionary<string, IModule>();
    public int Count=>_modules.Count;
    public ModuleActivator(IServiceProvider provider)
    {
        _provider=provider;
        _modConfigurations=provider.GetRequiredService<IConfiguration>().GetSection("MATSys:Modules").GetChildren().ToDictionary(x=>x["Alias"]);   
        _moduleFactory=_provider.GetRequiredService<IModuleFactory>();     
    }
    public IModule Create(string key)
    {
        if (_modules.ContainsKey(key))
        {
            return _modules[key];
        }
        else
        {
            var mod=_moduleFactory.CreateModule(_modConfigurations[key]);
            mod.IsDisposed +=ModuleDisposed;
            _modules.Add(key,mod);
            return mod;
        }        
    }

    private  void ModuleDisposed(object sender, EventArgs e)
    {
        _modules.Remove(((IModule)sender).Alias);
    }
    

}