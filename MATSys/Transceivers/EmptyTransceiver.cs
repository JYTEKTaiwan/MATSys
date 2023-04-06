
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MATSys.Plugins
{
    /// <summary>
    /// Default instance for transceiver, do nothing
    /// </summary>
    public sealed class EmptyTransceiver : ITransceiver
    {
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();
        public string Alias => nameof(EmptyTransceiver);

        public event ITransceiver.RequestFiredEvent? OnNewRequest;

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyTransceiver)} is initiated");
        }

        public void Load(object configuration)
        {
            _logger.Info($"{nameof(EmptyTransceiver)} is initiated");
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