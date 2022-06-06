using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface IDataBusFactory
    {
        IDataBus CreatePublisher(IConfigurationSection section);
    }
}