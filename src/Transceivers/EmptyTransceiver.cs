
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

        /// <summary>
        /// Event for the new coming request
        /// </summary>
        public event ITransceiver.RequestFiredEvent? OnNewRequest{ add{} remove{} }

        /// <summary>
        /// Name of the Service
        /// </summary>
        public string Alias { get; set; } = nameof(EmptyTransceiver);

        /// <summary>
        /// Load the configuration from file
        /// </summary>
        /// <param name="section">section of configuration filr</param>
        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyTransceiver)} is initiated");
        }
        /// <summary>
        /// load the configuration from object
        /// </summary>
        /// <param name="configuration">configuration object</param>

        public void Load(object configuration)
        {
            _logger.Info($"{nameof(EmptyTransceiver)} is initiated");
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
        /// <summary>
        /// Dispose the instance
        /// </summary> 
        public void Dispose()
        {
            GC.Collect();
        }
    }
}