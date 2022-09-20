
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MATSys.Plugins
{
    public sealed class EmptyTransceiver : ITransceiver
    {
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();
        public string Name => nameof(EmptyTransceiver);

        public event ITransceiver.RequestFiredEvent? OnNewRequest;

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyTransceiver)} is initiated");
        }

        public void Load(object configuration)
        {
            _logger.Info($"{nameof(EmptyTransceiver)} is initiated");
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