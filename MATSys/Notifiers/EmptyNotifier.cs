

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
            var json = MATSys.Utilities.Serializer.Serialize(data, false);
            OnNotify?.Invoke(json);
        }

        /// <summary>
        /// Export the service insatnce into JObject format
        /// </summary>
        /// <returns>JObject instance</returns>

#if NET6_0_OR_GREATER || NETSTANDARD2_0
        /// <summary>
        /// Export the service insatnce into JObject format
        /// </summary>
        /// <returns>JObject instance</returns>
        public System.Text.Json.Nodes.JsonObject Export()
        {
            return new System.Text.Json.Nodes.JsonObject();
        }
#elif NET35
        /// <summary>
        /// Export the service insatnce into JObject format
        /// </summary>
        /// <returns>JObject instance</returns>
        public Newtonsoft.Json.Linq.JObject Export()
        {
            return new Newtonsoft.Json.Linq.JObject();
        }
#endif
        /// <summary>
        /// Export the service instance into json format
        /// </summary>
        /// <param name="indented">In indented format</param>
        /// <returns>string</returns>

        public string Export(bool indented = true)
        {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
            return Export().ToJsonString(new System.Text.Json.JsonSerializerOptions() { WriteIndented = indented });
#elif NET35            
            if (indented) return Export().ToString(Newtonsoft.Json.Formatting.Indented);
            else return Export().ToString(Newtonsoft.Json.Formatting.None);

#else
#endif
        }
        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
        }
    }
}