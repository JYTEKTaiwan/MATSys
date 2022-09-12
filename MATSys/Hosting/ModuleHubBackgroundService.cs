using MATSys.Commands;
using MATSys.Factories;
using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NLog;
using System.Xml.Linq;

namespace MATSys
{
    public sealed class ModuleHubBackgroundService : BackgroundService
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly IConfiguration _config;
        private readonly ITransceiver _transceiver;
        private readonly AutoTestScheduler _scheduler;
        private readonly ILogger _logger;
        private CancellationTokenSource cts=new CancellationTokenSource();
        public ModuleCollection Modules { get; } = new ModuleCollection();
        public ModuleHubBackgroundService(IServiceProvider services)
        {
            try
            {                
                _logger = LogManager.GetCurrentClassLogger();
                _config = services.GetRequiredService<IConfiguration>().GetSection("MATSys");
                _moduleFactory = services.GetRequiredService<IModuleFactory>();
                foreach (var item in _config.GetSection("Modules").GetChildren())
                {
                    Modules.Add(item.GetValue<string>("Name"),_moduleFactory.CreateModule(item));
                }                
                _transceiver = services.GetRequiredService<ITransceiverFactory>().CreateTransceiver(_config.GetSection("Transceiver"));
                _transceiver.OnNewRequest += _transceiver_OnNewRequest;
                _scheduler=services.GetRequiredService<AutoTestScheduler>();
            }
            catch (Exception ex)
            {
                throw new Exception($"ModuleHub Initialzation failed", ex);
            }
        }
        private string _transceiver_OnNewRequest(object sender, string commandObjectInJson)
        {
            //format is {Name}:{command in json formt}
            var name = commandObjectInJson.Split(':')[0];
            var cmd = commandObjectInJson.Split(':')[1];
            return ExecuteCommand(name, cmd);
        }

        public string ExecuteCommand(string name, ICommand cmd)
        {
            //return ExecuteAsync(name, cmd).Result;
            return Modules[name].Execute(cmd);
        }
        public string ExecuteCommand(string name, string cmd)
        {
            //return ExecuteAsync(name, cmd).Result;
            return Modules[name].Execute(cmd);
        }

        public void RunTest(int iteration)
        {
            cts = new CancellationTokenSource();
            Task.Run(() => AutoTesting(iteration,cts.Token));
        }
        public void StopTest()
        {
            cts.Cancel();
        }
        private void AutoTesting(int iteration,CancellationToken token)
        {
            _scheduler.SingleTest();
            int cnt = 1;
            while (!cts.IsCancellationRequested)
            {
                if (cnt== iteration)
                {
                    cts.Cancel();
                }
                else if (!_scheduler.IsAvailable)
                {
                    _scheduler.SingleTest();
                    Interlocked.Increment(ref cnt);
                }
                else
                {
                    SpinWait.SpinUntil(() => false, 5);
                }
                
            }

        }
        private void Setup(CancellationToken stoppingToken)
        {
            foreach (var item in Modules.Values)
            {
                item.StartService(stoppingToken);
            }

        }

        private void Cleanup()
        {
            foreach (var item in Modules.Values)
            {
                item.StopService();
            }

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async() =>
            {
                Setup(stoppingToken);
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (SpinWait.SpinUntil(() => _scheduler.IsAvailable, 5))
                    {
                        var currentTestItem = await _scheduler.Dequeue(stoppingToken);
                        var response=ExecuteCommand(currentTestItem.ModuleName, currentTestItem.Command);
                    }
                }
                Cleanup();
            });
        }
    }

    public sealed class ModuleCollection : Dictionary<string,IModule>
    {
        public ModuleCollection() : base()
        {
        }

    }

}