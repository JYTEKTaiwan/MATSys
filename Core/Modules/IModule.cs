using Microsoft.Extensions.Configuration;

namespace MATSys
{
    public interface IModule
    {
        string Name { get;  }

        void Load(IConfigurationSection section);

        Task RunAsync(CancellationToken token);

        void Stop();
    }
}