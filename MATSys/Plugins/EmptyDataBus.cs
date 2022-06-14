using Microsoft.Extensions.Configuration;

namespace MATSys.Plugins
{
    public sealed class EmptyDataBus : IDataBus
    {
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name => nameof(EmptyDataBus);

        public event IDataBus.NewDataEvent? OnNewDataReadyEvent;

        public object? GetData(int timeoutInMilliseconds = 1000)
        {
            return null!;
        }

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyDataBus)} is initiated");
        }

        public void Publish(object data)
        {
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