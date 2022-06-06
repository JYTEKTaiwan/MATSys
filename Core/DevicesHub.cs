using MATSys.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace MATSys
{
    public sealed class DevicesHub
    {
        private static Lazy<DevicesHub> lazy = new Lazy<DevicesHub>(() => new DevicesHub());
        private readonly IHost host;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private volatile bool isRunning = false;
        public DeviceCollection Devices { get; } = new DeviceCollection();
        public static DevicesHub Instance => lazy.Value;

        private DevicesHub()
        {
            try
            {
                host = Host.CreateDefaultBuilder()
                    .ConfigureServices(services => services
                    .AddSingleton<IDeviceFactory, DeviceFactory>()
                    .AddSingleton<IDataRecorderFactory, DataRecorderFactory>()
                    .AddSingleton<IDataBusFactory, DataBusFactory>()
                    .AddSingleton<ICommandServerFactory, CommandServerFactory>()
                    )
                    .ConfigureLogging(logging => logging
                    .AddNLog()
                    )
                    .Build();
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
                    host.RunAsync().Wait(100);
                    var devFactory = host.Services.GetRequiredService<IDeviceFactory>() as DeviceFactory;
                    foreach (var item in devFactory!.DeviceInfos)
                    {
                       var dev = devFactory.CreateDevice(item);
                        dev!.RunAsync(cts.Token);
                        Devices.Add(dev);
                    }
                    isRunning = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"DeviceHub starts failed", ex);
            }
        }

        public void Stop()
        {
            try
            {
                if (isRunning)
                {
                    cts.Cancel();
                    foreach (var item in Devices)
                    {
                        item.Stop();
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

    public sealed class DeviceCollection : List<IDevice>
    {
        public DeviceCollection() : base()
        {
        }

        public IDevice this[string name] => this.FirstOrDefault(x => x.Name == name)!;
    }
}