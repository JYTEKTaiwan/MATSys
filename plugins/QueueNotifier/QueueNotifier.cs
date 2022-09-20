using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Channels;

namespace MATSys.Plugins
{
    internal sealed class QueueNotifier : INotifier
    {
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();
        private Channel<string>? _ch;
        private QueueNotifierConfiguration? _config;

        public string Name => nameof(QueueNotifier);

        public event INotifier.NotifyEvent? OnNotify;

        public object? GetData(int timeoutInMilliseconds = 1000)
        {
            var task = _ch!.Reader.ReadAsync().AsTask();
            var to = task.Wait(timeoutInMilliseconds);
            return to ? task.Result : null;
        }

        public void Load(IConfigurationSection section)
        {
            _config = section.Get<QueueNotifierConfiguration>();
            _logger = _config.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger(); ;
            _logger.Info("QueueNotifier is initiated");
        }

        public void Load(object configuration)
        {
            _config = (QueueNotifierConfiguration)configuration;
            _logger = _config.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger(); ;
            _logger.Info("QueueNotifier is initiated");
        }

        public void Publish(object data)
        {
            _ch!.Writer.TryWrite(JsonConvert.SerializeObject(data));
            if (!_config!.DisableEvent)
            {
                OnNotify?.Invoke(JsonConvert.SerializeObject(data));
            }
        }

        public void StartService(CancellationToken token)
        {
            _ch = Channel.CreateBounded<string>(new BoundedChannelOptions(_config!.QueueLength) { FullMode = _config.Mode });
        }

        public void StopService()
        {
            _ch!.Writer.Complete();
        }
        public JObject Export()
        {
            return  JObject.FromObject(_config);

        }
        public string Export(Formatting format = Formatting.Indented)
        {
            return Export().ToString(Formatting.Indented);
        }


    }

    internal sealed class QueueNotifierConfiguration: IMATSysConfiguration
    {
        public string Type { get; set; } = "queue";
        public bool EnableLogging { get; set; } = false;
        public bool DisableEvent { get; set; } = true;
        public int QueueLength { get; set; } = 1000;
        public BoundedChannelFullMode Mode { get; set; } = BoundedChannelFullMode.DropOldest;
    }
}