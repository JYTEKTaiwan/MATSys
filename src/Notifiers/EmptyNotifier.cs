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


        /// <summary>
        /// Name of the Service
        /// </summary>
        public string Alias { get; set; } = nameof(EmptyNotifier);

        /// <summary>
        /// Event when new data is ready to notify
        /// </summary>
        public event INotifier.NotifyEvent? OnNotify;

        /// <summary>
        /// Get the latest data stored in the buffer. Return null if timeout
        /// </summary>
        /// <param name="timeoutInMilliseconds">Timeout value in milliseconds</param>

        public object? GetData(int timeoutInMilliseconds = 1000)
        {
            return null!;
        }
        /// <summary>
        /// Load the configuration from file
        /// </summary>
        /// <param name="section">section of configuration filr</param>

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyNotifier)} is initiated");
        }
        /// <summary>
        /// load the configuration from object
        /// </summary>
        /// <param name="configuration">configuration object</param>

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
        /// <summary>
        /// Start the service
        /// </summary>
        /// <param name="token">Stop token</param>

        public void StartPluginService(CancellationToken token)
        {
        }
        /// <summary>
        /// Stop service
        /// </summary>

        public void StopPluginService()
        {
        }
        /// <summary>
        /// Export the service insatnce into JObject format
        /// </summary>
        /// <returns>JObject instance</returns>

        public JsonObject Export()
        {
            return new JsonObject();
        }
        /// <summary>
        /// Export the service instance into json format
        /// </summary>
        /// <param name="indented">In indented format</param>
        /// <returns>string</returns>

        public string Export(bool indented = true)
        {
            return Export().ToJsonString(new JsonSerializerOptions() { WriteIndented = indented });
        }

        public void Dispose()
        {
            
        }
    }
}