using Microsoft.Extensions.Configuration;

namespace MATSys
{
    public interface IModule
    {
        string Name { get; }

        void Load(IConfigurationSection section);

        void StartService(CancellationToken token);

        void StopService();
    }
}