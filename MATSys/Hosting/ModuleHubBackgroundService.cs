using MATSys.Commands;
using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace MATSys
{
    public sealed class ModuleHubBackgroundService : BackgroundService
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly IConfiguration _config;
        private readonly ITransceiver _transceiver;
        public ModuleCollection Modules { get; } = new ModuleCollection();
        public ModuleHubBackgroundService(IServiceProvider services)
        {
            try
            {
                _config = services.GetRequiredService<IConfiguration>().GetSection("MATSys");
                _moduleFactory = services.GetRequiredService<IModuleFactory>();
                foreach (var item in _config.GetSection("Modules").GetChildren())
                {
                    Modules.Add(_moduleFactory.CreateModule(item));
                }                
                _transceiver = services.GetRequiredService<ITransceiverFactory>().CreateTransceiver(_config.GetSection("Transceiver"));
                _transceiver.OnNewRequest += _transceiver_OnNewRequest; ;
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
            return Execute(name, cmd);
        }

        public string Execute(string name, ICommand cmd)
        {
            if (Modules.Exists(x => x.Name == name))
            {
                return Modules[name].Execute(cmd);
            }
            else
            {
                return $"Module [{name}] is not Found";
            }
        }
        public string Execute(string name, string cmd)
        {
            if (Modules.Exists(x => x.Name == name))
            {
                return Modules[name].Execute(cmd);
            }
            else
            {
                return $"Module [{name}] is not Found";
            }
        }

        private void Setup(CancellationToken stoppingToken)
        {
            foreach (var item in Modules)
            {
                item.StartService(stoppingToken);
            }

        }

        private void Cleanup()
        {
            foreach (var item in Modules)
            {
                item.StopService();
            }

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                Setup(stoppingToken);
                while (!stoppingToken.IsCancellationRequested)
                {
                    Thread.Sleep(1);
                }
                Cleanup();
            });
        }
    }

    [JsonArray(AllowNullItems = true, NamingStrategyType = typeof(string))]
    public class AssemblyPathCollection
    {
        public string[] Modules { get; set; }
        public string[] Recorders { get; set; }
        public string[] Notifiers { get; set; }

        public string[] Transceivers { get; set; }
    }
    public sealed class ModuleCollection : List<IModule>
    {
        public ModuleCollection() : base()
        {
        }

        public IModule this[string name] => this.FirstOrDefault(x => x.Name == name)!;
    }

}