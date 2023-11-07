using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace MATSys.Plugins
{
    internal sealed class QueueNotifier : INotifier
    {
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();
        private Channel<string>? _ch;
        private QueueNotifierConfiguration? _config;

        public string Alias => nameof(QueueNotifier);

        string IService.Alias { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
            _ch!.Writer.TryWrite(JsonSerializer.Serialize(data));
            if (!_config!.DisableEvent)
            {
                OnNotify?.Invoke(JsonSerializer.Serialize(data));
            }
        }

        public void StartPluginService(CancellationToken token)
        {
            _ch = Channel.CreateBounded<string>(new BoundedChannelOptions(_config!.QueueLength) { FullMode = _config.Mode });
        }

        public void StopPluginService()
        {
            _ch!.Writer.Complete();
        }
        public JsonObject Export()
        {
            return JsonObject.Parse(JsonSerializer.Serialize(_config)).AsObject();

        }
        public string Export(bool indented = true)
        {
            return Export().ToJsonString(new JsonSerializerOptions() { WriteIndented = indented });
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class QueueNotifierConfiguration : IMATSysConfiguration
    {
        public string Type { get; set; } = "queue";
        public bool EnableLogging { get; set; } = false;
        public bool DisableEvent { get; set; } = true;
        public int QueueLength { get; set; } = 1000;
        public BoundedChannelFullMode Mode { get; set; } = BoundedChannelFullMode.DropOldest;
    }
}