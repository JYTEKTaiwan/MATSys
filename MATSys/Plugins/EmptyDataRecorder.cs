using Microsoft.Extensions.Configuration;

namespace MATSys.Plugins
{
    public sealed class EmptyDataRecorder : DataRecorderBase
    {        
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public override string Name => nameof(EmptyDataRecorder);       

        public override void StopService()
        {
        }

        public override Task StartServiceAsync(CancellationToken token)
        {            
            return Task.CompletedTask;
        }

        public override void Write(object data)
        {
        }

        public override Task WriteAsync(object data)
        {
            return Task.CompletedTask;
        }

        public override void Load(IConfigurationSection section)
        {            
            _logger.Info($"{nameof(EmptyDataRecorder)} is initiated");
        }

       
        
    }
}