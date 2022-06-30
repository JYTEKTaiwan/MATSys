using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public interface IDataRecorderFactory
    {
        IRecorder CreateRecorder(IConfigurationSection section);
        
    }
}