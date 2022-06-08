using Microsoft.Extensions.Configuration;

namespace MATSys
{
    public interface IModule
    {
        string Name { get; }

        void Load(IConfigurationSection section);

        Task StartServiceAsync(CancellationToken token);

        void StopService();
    }
}