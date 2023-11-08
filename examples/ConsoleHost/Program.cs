using Microsoft.Extensions.Hosting;
using MATSys;
using MATSys.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MATSys.Commands;

namespace ConsoleHost
{
    internal class ConsoleHost
    {
        internal static void Main()
        {

            Console.WriteLine("Hello, World!");
            var b = Host.CreateDefaultBuilder();
            b.ConfigureAppConfiguration(app => app.AddConfigurationInMATSys());
            b.ConfigureLogging(log => log.AddNlogInMATSys());
            b.ConfigureServices(s => s.AddMATSysService());

            var host = b.Build();
            host.StartAsync();

            Thread.Sleep(1000);

            var a = host.Services.GetRequiredService<ModuleActivator>().Create("Dev1");
            a.Execute(CommandBase.Create("Delay", 3000));
            a.Execute(CommandBase.Create("Delay", 1000));
            Console.WriteLine(a.Alias);

            Thread.Sleep(5000);
            host.StopAsync();

            Console.WriteLine("DONE");





        }
    }
    internal class TestDevice : ModuleBase
    {
        [MATSysCommand]
        public void Delay(int a)
        {
            Console.WriteLine($"[{DateTime.Now}]Start - {a}");
            Thread.Sleep(a);
            Console.WriteLine($"[{DateTime.Now}]Done - {a}");
        }


    }

}




