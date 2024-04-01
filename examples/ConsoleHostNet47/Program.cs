using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MATSys;
using MATSys.Commands;
using Microsoft.Extensions.Hosting;
using MATSys.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Xml.Schema;
using MockModulesLibrary;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Runtime.InteropServices;
using System.Globalization;
using Microsoft.Extensions.DependencyModel;
using MockModulesLibrary;

namespace ConsoleHostNet47
{
    internal class Program
    {

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
}
