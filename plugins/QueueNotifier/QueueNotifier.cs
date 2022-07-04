using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Channels;

namespace MATSys.Plugins
{
    internal sealed class QueueNotifier : INotifier
    {
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
        private Channel<string>? _ch;
        private QueueDataBusConfiguration? config;

        public string Name => nameof(QueueNotifier);

        public event INotifier.NewDataEvent? OnNewDataReadyEvent;

        public object? GetData(int timeoutInMilliseconds = 1000)
        {
            var task = _ch!.Reader.ReadAsync().AsTask();
            var to = task.Wait(timeoutInMilliseconds);
            return to ? task.Result : null;
        }

        public void Load(IConfigurationSection section)
        {
            config = section.Get<QueueDataBusConfiguration>();
            _logger.Info("QueueDataBus is initiated");
        }
        public void LoadFromObject(object configuration)
        {
            config = configuration as QueueDataBusConfiguration;
            _logger.Info("QueueDataBus is initiated");
        }
        public void Publish(object data)
        {
            _ch!.Writer.TryWrite(JsonConvert.SerializeObject(data));
            if (!config!.DisableEvent)
            {
                OnNewDataReadyEvent?.Invoke(JsonConvert.SerializeObject(data));
            }
        }

        public void StartService(CancellationToken token)
        {
            _ch = Channel.CreateBounded<string>(new BoundedChannelOptions(config!.QueueLength) { FullMode = config.Mode });

        }

        public void StopService()
        {
            _ch!.Writer.Complete();
        }
    }

    internal sealed class QueueDataBusConfiguration
    {
        public bool DisableEvent { get; set; } = true;
        public int QueueLength { get; set; } = 1000;
        public BoundedChannelFullMode Mode { get; set; } = BoundedChannelFullMode.DropOldest;
    }
}