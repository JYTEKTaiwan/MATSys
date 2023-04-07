// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;
using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace InteractionWithHost_net472
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder().UseMATSys().Build();
            host.RunAsync().Wait(1000); ;

            var dev = host.GetMATSysRunner();


            dev.AfterTestItemStops += (item, res) => { Console.WriteLine($"{res.ToJsonString()}"); };
            dev.RunTest();


            Console.WriteLine("PRESS ANY KEY TO EXIT");

            Console.ReadKey();
            host.StopAsync();
        }
    }


    public class TestDevice : ModuleBase
    {


        public override void Load(IConfigurationSection section)
        {
        }

        public override void Load(object configuration)
        {

        }

        [MATSysCommand]
        public string Method(string c)
        {
            return c;
        }
    }
}



