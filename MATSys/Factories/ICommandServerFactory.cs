using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface ICommandServerFactory
    {
        ICommandServer CreateCommandStream(IConfigurationSection section);
    }
}