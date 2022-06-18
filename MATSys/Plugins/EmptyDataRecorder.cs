
using Microsoft.Extensions.Configuration;

namespace MATSys.Plugins
{
    public sealed class EmptyDataRecorder : IDataRecorder
    {
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name => nameof(EmptyDataRecorder);

        public void StopService()
        {
        }

        public void StartService(CancellationToken token)
        {
        }

        public void Write(object data)
        {
        }

        public Task WriteAsync(object data)
        {
            return Task.CompletedTask;
        }

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyDataRecorder)} is initiated");
        }



    }
}