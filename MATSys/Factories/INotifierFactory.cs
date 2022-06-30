using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface INotifierFactory
    {
        INotifier CreateNotifier(IConfigurationSection section);
    }
}