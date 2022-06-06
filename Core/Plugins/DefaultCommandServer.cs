using Microsoft.Extensions.Configuration;

namespace MATSys.Plugins
{
    internal sealed class DefaultCommandServer : ICommandServer
    {
        public string Name => nameof(DefaultCommandServer);
        public event ICommandServer.CommandReadyEvent OnCommandReady;

        public void Load(IConfigurationSection section)
        {
        }

        public Task RunAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public void Stop()
        {
        }
    }
}