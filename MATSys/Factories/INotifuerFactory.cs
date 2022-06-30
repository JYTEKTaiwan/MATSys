using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface INotifuerFactory
    {
        INotifier CreateDataBus(IConfigurationSection section);
    }
}