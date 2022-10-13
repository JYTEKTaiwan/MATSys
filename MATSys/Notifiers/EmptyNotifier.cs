
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MATSys.Plugins
{
    /// <summary>
    /// Default instance for notifier, only Publish method is valid
    /// </summary>
    public sealed class EmptyNotifier : INotifier
    {
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();

        /// <summary>
        /// Name of the instance
        /// </summary>
        /// <returns>"EmptyNotifier"</returns>
        public string Name => nameof(EmptyNotifier);

        /// <summary>
        /// Event when data is ready to publish
        /// </summary>
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
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            OnNotify?.Invoke(json);
        }

        public void StartService(CancellationToken token)
        {
        }

        public void StopService()
        {
        }
        public JObject Export()
        {
            return new JObject();
        }
        public string Export(Formatting format = Formatting.Indented)
        {
            return Export().ToString(Formatting.Indented);
        }

    }
}