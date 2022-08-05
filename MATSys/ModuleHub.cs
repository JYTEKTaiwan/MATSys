using MATSys.Commands;
using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace MATSys
{
    public sealed class ModuleHub
    {
        private static Lazy<ModuleHub> lazy = new Lazy<ModuleHub>(() => new ModuleHub());
        private readonly IHost host;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private volatile bool isRunning = false;
        public ModuleCollection Modules { get; } = new ModuleCollection();
        public static ModuleHub Instance => lazy.Value;

        private ModuleHub()
        {
            try
            {
                host = Host.CreateDefaultBuilder()
                    .ConfigureServices(services => services
                    .AddSingleton<IModuleFactory, ModuleFactory>()
                    .AddSingleton<IRecorderFactory, RecorderFactory>()
                    .AddSingleton<INotifierFactory, NotifierFactory>()
                    .AddSingleton<ITransceiverFactory, TransceiverFactory>()
                    )
                    .ConfigureLogging(logging => logging
                    .AddNLog()
                    )
                    .Build();
                //configure NLog environment for the first time
                var config = host.Services.GetService<IConfiguration>()!;
                NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
            }
            catch (Exception ex)
            {
                throw new Exception($"DeviceHub Initialzation failed", ex);
            }
        }

        public void Start()
        {
            try
            {
                if (!isRunning)
                {
                    //start the host and delay 500ms
                    host.RunAsync().Wait(500);
                    var devFactory = host.Services.GetRequiredService<IModuleFactory>() as ModuleFactory;

                    foreach (var item in devFactory!.DeviceInfos)
                    {
                        var dev = devFactory.CreateDevice(item);

                        dev!.StartService(cts.Token);
                        Modules.Add(dev);
                    }
                    isRunning = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"DeviceHub starts failed", ex);
            }
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

        public void Stop()
        {
            try
            {
                if (isRunning)
                {
                    cts.Cancel();
                    foreach (var item in Modules)
                    {
                        item.StopService();
                    }
                    host.StopAsync();
                    isRunning = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"DeviceHub stops failed", ex);
            }
        }
    }

    public sealed class ModuleCollection : List<IModule>
    {
        public ModuleCollection() : base()
        {
        }

        public IModule this[string name] => this.FirstOrDefault(x => x.Name == name)!;
    }
}