using MATSys;
using MATSys.Commands;
using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using MATSys.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Reflection;
using MATSys.Factories;

namespace ConsoleHostNet47
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            
            Console.WriteLine("Hello, World!");
            var b = Host.CreateDefaultBuilder();

            b.ConfigureHostConfiguration(h => h.AddConfigurationInMATSys());
            b.ConfigureServices(s =>
            s.AddMATSysService()
            );

            var host = b.Build();
            host.StartAsync();
            
            Thread.Sleep(1000);
            var dev = host.Services.GetModule("Dev1");
            var dev2 = host.Services.GetModule("Mod1");
            Console.WriteLine($"{dev.Alias}_{dev.GetHashCode()}");
            Console.WriteLine($"{dev2.Alias}_{dev2.GetHashCode()}");
            dev.Execute("Delay", 1000);
            host.StopAsync();
            host.Dispose();
            Console.WriteLine("DONE");
            Console.ReadKey();
        }
    }


}
