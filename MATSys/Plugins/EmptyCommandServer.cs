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

        public  void StartService(CancellationToken token)
        {
            
        }

        public void StopService()
        {
        }
    }
}