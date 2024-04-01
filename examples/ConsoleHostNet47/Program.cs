using MATSys;
using MATSys.Commands;
using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;

namespace ConsoleHostNet47
{
    internal class Program
    {
        public delegate IModule ModuleResolver(IConfigurationSection section);
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var b = Host.CreateDefaultBuilder();

            b.ConfigureHostConfiguration(h => h.AddConfigurationInMATSys());
            b.ConfigureServices(s => s.AddMATSysService());

            var host = b.Build();
            host.StartAsync();

            Thread.Sleep(1000);

            var dev = host.Services.GetModule("Dev1");
            var mod = host.Services.GetModule("Mod1");

            dev.Execute("Delay", 1000);
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

    internal class TestDevice2 : ModuleBase
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
