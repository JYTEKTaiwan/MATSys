// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;
using MATSys.Hosting;
using MATSys.Hosting.Scripting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Nodes;
using System.Text.Json;
using MATSys.Factories;

namespace InteractionWithHost
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder().UseMATSys().Build();

            host.RunAsync().Wait(1000); 

            var runner = host.GetMATSysRunner();
            runner.AfterTestItemStops += (item, res) =>
            {
                //event is fired after executeing test item;
                Console.WriteLine($"{res.ToJsonString()}");
            };
            runner.AfterScriptStops += (res) =>
            {
                foreach (var item in res)
                {
                    //Console.WriteLine(item.ToJsonString());
                }
            };
            runner.RunTestAsync();

            //var a = runner.RunTest(1);
            //foreach (var item in a.ToArray())
            //{
            //    Console.WriteLine(item.ToJsonString());
            //}
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
    internal class MyTestPackage : TestPackageBase
    {
        IModule mod;

        [TestItemParameter(typeof(WhoAreYouArgs))]
        public JsonNode WhoAreYou(JsonNode node)
        {
            var args = node.Deserialize<WhoAreYouArgs>();
            args.Age++;
            return JsonSerializer.SerializeToNode(args);
        }

        [TestItemParameter(typeof(InitializeArgs))]
        public JsonNode Initialize(JsonNode node)
        {
            var args = node.Deserialize<InitializeArgs>();
            var modFactory = this.Provider.GetRequiredService<IModuleFactory>();
            var modsinfo = this.Provider.GetAllModuleInfos();
            mod = modFactory.CreateModule(modsinfo["Dev1"]);
            return JsonSerializer.SerializeToNode(args);
        }

        [TestItemParameter(typeof(CloseArgs))]
        public JsonNode Close(JsonNode node)
        {
            var args = node.Deserialize<CloseArgs>();
            return JsonSerializer.SerializeToNode(args);
        }

        [TestItemParameter(typeof(DoArgs))]
        public JsonNode Do(JsonNode node)
        {
            var str=mod.Execute(CommandBase.Create("Method", "HELLO"));
            return JsonNode.Parse(str);
        }
        internal class InitializeArgs
        {

        }
        internal class CloseArgs
        {

        }
        internal class DoArgs
        {

        }

        internal class WhoAreYouArgs
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}




