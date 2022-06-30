
using Microsoft.Extensions.Configuration;

namespace MATSys.Plugins
{
    public sealed class EmptyRecorder : IRecorder
    {
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

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



    }
}