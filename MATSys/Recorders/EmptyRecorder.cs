
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MATSys.Plugins
{
    /// <summary>
    /// Default instance for recorder, do nothing
    /// </summary>
    public sealed class EmptyRecorder : IRecorder
    {
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();

        public string Name => nameof(EmptyRecorder);
        public void StopService()
        {
        }

        public void StartService(CancellationToken token)
        {
        }

        public void Write(object data)
        {
        }

        public async Task WriteAsync(object data)
        {
            await Task.CompletedTask;
        }

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyRecorder)} is initiated");
        }

        public void Load(object configuration)
        {
            _logger.Info($"{nameof(EmptyRecorder)} is initiated");
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