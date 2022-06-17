using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface IDataBusFactory
    {
        IDataBus CreateDataBus(IConfigurationSection section);
    }
}