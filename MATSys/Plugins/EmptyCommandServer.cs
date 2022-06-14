using Microsoft.Extensions.Configuration;

namespace MATSys.Plugins
{
    public sealed class EmptyCommandServer : ICommandServer
    {  
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
        public string Name => nameof(EmptyCommandServer);

        public event ICommandServer.CommandReadyEvent? OnCommandReady;

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyCommandServer)} is initiated");
        }

        public Task StartServiceAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public void StopService()
        {
        }
    }
}