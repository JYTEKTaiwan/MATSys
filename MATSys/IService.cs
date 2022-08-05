using Microsoft.Extensions.Configuration;

namespace MATSys
{
    public interface IService
    {
        string Name { get; }

        void Load(IConfigurationSection section);

        void Load(object configuration);

        void StartService(CancellationToken token);

        void StopService();
    }
}
