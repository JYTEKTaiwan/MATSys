using Microsoft.Extensions.Hosting;
using MATSys;
using MATSys.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleHost
{
    internal class ConsoleHost
    {
        internal static void Main()
        {

            Console.WriteLine("Hello, World!");
            var b = Host.CreateDefaultBuilder();
            b.ConfigureAppConfiguration(app=>app.AddConfigurationInMATSys());
            b.ConfigureLogging(log => log.AddNlogInMATSys());
            b.ConfigureServices(s => s.AddMATSysService());

            var host = b.Build();
            host.StartAsync();

            Thread.Sleep(1000);

            var a = host.Services.GetRequiredService<IModule>();

            Console.WriteLine(a.Alias);
            host.StopAsync();

            Console.WriteLine("DONE");





        }
    }
    internal class TestDevice : ModuleBase
    {

        [ID("Dev1")]
        public TestDevice(IServiceProvider provider) : base(provider)
        {
        }
    }

}




