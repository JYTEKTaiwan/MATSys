
/* Unmerged change from project 'MATSys (netstandard2.0)'
Before:
using Microsoft.Extensions.Configuration;
After:
using MATSys;
using MATSys;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
*/

/* Unmerged change from project 'MATSys (netstandard2.0)'
Before:
using MATSys.Modules;
After:
using MATSys;
using MATSys;
using MATSys.Modules;
*/
using Microsoft.Extensions.Configuration;

namespace MATSys
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
    }
}