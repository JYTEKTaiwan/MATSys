using Microsoft.Extensions.Configuration;

namespace MATSys.Plugins
{
    public sealed class EmptyNotifier : INotifier
    {
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name => nameof(EmptyNotifier);

        public event INotifier.NewDataEvent? OnNewDataReadyEvent;

        public object? GetData(int timeoutInMilliseconds = 1000)
        {
            return null!;
        }

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyNotifier)} is initiated");
        }

        public void Publish(object data)
        {
        }

        public void StartService(CancellationToken token)
        {
        }

        public void StopService()
        {
        }
    }
}