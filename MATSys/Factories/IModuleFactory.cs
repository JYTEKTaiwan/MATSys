using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface IModuleFactory
    {
        IModule CreateDevice(IConfigurationSection info);

    }
}