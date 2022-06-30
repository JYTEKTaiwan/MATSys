using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface INotifierFactory
    {
        INotifier CreateDataBus(IConfigurationSection section);
    }
}