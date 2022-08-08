using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface IModuleFactory
    {
        IModule CreateModule(IConfigurationSection info);

    }
}