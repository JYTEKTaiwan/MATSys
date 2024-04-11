
namespace MATSys.Plugins
{
    /// <summary>
    /// Default instance for recorder, do nothing
    /// </summary>
    public sealed class EmptyRecorder : IRecorder
    {

        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();
        private bool disposedValue;

        public event ServiceExceptionFired ExceptionFired;
        public event ServiceDisposed Disposed;

        public object Configuration { get; set; } = new object();

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

        public void Configure()
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
#elif NET35||NET462
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
#elif NET35||NET462            
            if (indented) return Export().ToString(Newtonsoft.Json.Formatting.Indented);
            else return Export().ToString(Newtonsoft.Json.Formatting.None);


#else
#endif
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Disposed?.Invoke(this, EventArgs.Empty);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EmptyRecorder()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}