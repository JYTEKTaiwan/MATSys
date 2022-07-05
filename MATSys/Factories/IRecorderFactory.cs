using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface IRecorderFactory
    {
        IRecorder CreateRecorder(IConfigurationSection section);
    }
}