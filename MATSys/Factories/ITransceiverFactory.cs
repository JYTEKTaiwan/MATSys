using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface ITransceiverFactory
    {
        ITransceiver CreateCommandStream(IConfigurationSection section);
    }
}