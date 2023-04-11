
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MATSys.Plugins
{
    /// <summary>
    /// Default instance for recorder, do nothing
    /// </summary>
    public sealed class EmptyRecorder : IRecorder
    {
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();
        /// <summary>
        /// Name of the Service
        /// </summary>

        public string Alias { get; set; } = nameof(EmptyRecorder);
        /// <summary>
        /// Stop service
        /// </summary>

        public void StopPluginService()
        {
        }
        /// <summary>
        /// Start the service
        /// </summary>
        /// <param name="token">Stop token</param>

        public void StartPluginService(CancellationToken token)
        {
        }
        /// <summary>
        /// Write data to the instance
        /// </summary>
        /// <param name="data">Data to be written</param>

        public void Write(object data)
        {
        }
        /// <summary>
        /// Write data to the instance asynchronuously
        /// </summary>
        /// <param name="data">Data to be written</param>

        public async Task WriteAsync(object data)
        {
            await Task.CompletedTask;
        }
        /// <summary>
        /// Load the configuration from file
        /// </summary>
        /// <param name="section">section of configuration filr</param>

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyRecorder)} is initiated");
        }
        /// <summary>
        /// load the configuration from object
        /// </summary>
        /// <param name="configuration">configuration object</param>

        public void Load(object configuration)
        {
            _logger.Info($"{nameof(EmptyRecorder)} is initiated");
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

    }
}