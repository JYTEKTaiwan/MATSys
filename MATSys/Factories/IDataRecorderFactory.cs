using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface IDataRecorderFactory
    {
        IDataRecorder CreateRecorder(IConfigurationSection section);
    }
}