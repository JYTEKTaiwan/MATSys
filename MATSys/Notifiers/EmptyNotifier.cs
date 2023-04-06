using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MATSys.Plugins
{
    /// <summary>
    /// Default instance for notifier, only Publish method is valid
    /// </summary>
    public sealed class EmptyNotifier : INotifier
    {
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();


        public string Alias => nameof(EmptyNotifier);


        public event INotifier.NotifyEvent? OnNotify;


        public object? GetData(int timeoutInMilliseconds = 1000)
        {
            return null!;
        }

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyNotifier)} is initiated");
        }

        public void Load(object configuration)
        {
            _logger.Info($"{nameof(EmptyNotifier)} is initiated");
        }

        /// <summary>
        /// Publish the data in json format
        /// </summary>
        /// <param name="data">data need to publish</param>
        public void Publish(object data)
        {
            var json = JsonSerializer.Serialize(data);
            OnNotify?.Invoke(json);
        }

        public void StartPluginService(CancellationToken token)
        {
        }

        public void StopPluginService()
        {
        }
        public JsonObject Export()
        {
            return new JsonObject();
        }
        public string Export(bool indented = true)
        {
            return Export().ToJsonString(new JsonSerializerOptions() { WriteIndented = indented });
        }

    }
}