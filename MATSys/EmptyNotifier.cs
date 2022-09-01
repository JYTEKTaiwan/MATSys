
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
    public sealed class EmptyNotifier : INotifier
    {
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();

        public string Name => nameof(EmptyNotifier);


        public event INotifier.NotifyEvent? OnNotify;

        public object? GetData(int timeoutInMilliseconds = 1000)
        {
            return null!;
        }

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyNotifier)} is initiated");
        }

        public void Load(object configuration)
        {
            _logger.Info($"{nameof(EmptyNotifier)} is initiated");
        }

        public void Publish(object data)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            OnNotify.Invoke(json);
        }

        public void StartService(CancellationToken token)
        {
        }

        public void StopService()
        {
        }
    }
}