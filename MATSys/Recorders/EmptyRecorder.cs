
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
        /// Write data to the instance
        /// </summary>
        /// <param name="data">Data to be written</param>

        public void Write(object data)
        {
        }
#if NET6_0_OR_GREATER || NETSTANDARD2_0
        /// <summary>
        /// Write data to the instance asynchronuously
        /// </summary>
        /// <param name="data">Data to be written</param>

        public async Task WriteAsync(object data)
        {
            await Task.CompletedTask;
        }
#endif


        /// <summary>
        /// load the configuration from object
        /// </summary>
        /// <param name="configuration">configuration object</param>

        public void Load(object configuration)
        {
            _logger.Info($"{nameof(EmptyRecorder)} is initiated");
        }
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
        /// Dispose the instance
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
        }
    }
}