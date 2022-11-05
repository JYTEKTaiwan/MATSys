
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
        public JsonObject Export()
        {            
            return new JsonObject();
        }
        public string Export(bool indented=true)
        {
            return Export().ToJsonString(new JsonSerializerOptions() { WriteIndented=indented});
        }

    }
}