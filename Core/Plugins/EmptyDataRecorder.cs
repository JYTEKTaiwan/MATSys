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

        public Task StartServiceAsync(CancellationToken token)
        {
            return Task.CompletedTask;
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
            _logger.Info("DefaultDataRecorder is initiated");
        }
    }
}