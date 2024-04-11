using MATSys;
using MATSys.Commands;
using MATSys.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;

namespace ConsoleHostNet80
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var b = Host.CreateDefaultBuilder();

            b.ConfigureHostConfiguration(h => h.UseConfigurationInMATSys());
            b.ConfigureServices((c,s) =>
            s.AddMATSysService(c)
            );

            var host = b.Build();
            host.StartAsync();

            Thread.Sleep(1000);
            var dev = host.Services.GetModule("Dev1");
            var dev2 = host.Services.GetModule("Dev1");
            var dev3 = host.Services.GetModule("Mod2");

            Console.WriteLine($"{dev.Alias}_{dev.GetHashCode()}");
            Console.WriteLine($"{dev2.Alias}_{dev2.GetHashCode()}");
            dev.ExecuteRaw("{\"Delay\":[1000]}", out var a);
            Console.WriteLine(a);
            host.StopAsync();
            host.Dispose();
            Console.WriteLine("DONE");
            Console.ReadKey();
        }
    }

    internal class TestDevice : ModuleBase
    {
        public TestDevice()
        {
            this.Disposed += TestDevice_Disposed;
        }

        private void TestDevice_Disposed(object sender, EventArgs e)
        {
            Console.WriteLine($"{Alias} is disposed");
        }

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
