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
        static async Task Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder().UseMATSys().Build();

            host.RunAsync().Wait(1000); 

            var runner = host.GetMATSysRunner();
            runner.BeforeScriptStarts += (script) => 
            {
                foreach (var item in script.AsArray())
                {
                    Console.WriteLine($"[BeforeScript]{item.ToJsonString()}");
                }
            };
            runner.BeforeTestItemStarts += (item) => 
            {
                Console.WriteLine($"[BeforeItem]{runner.CurrentItem.ToJsonString()}");
            };
            runner.AfterTestItemStops += (item, res) =>
            {
                //event is fired after executeing test item;
                Console.WriteLine($"[AfterItem-{item.ToJsonString()}]{res.ToJsonString()}");
            };
            runner.AfterScriptStops += (res,status) =>
            {
                foreach (var item in res)
                {
                    Console.WriteLine($"[AfterScript]{item.ToJsonString()}");
                }
            };
            var t=runner.RunTestAsync();
            Thread.Sleep(1500);
            runner.StopTest();
            Console.WriteLine(runner.Status);
            Console.WriteLine("PRESS ANY KEY TO EXIT");
            //var result=await t;
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
        public IResult WhoAreYou(JsonNode node)
        {
            var args = node.Deserialize<WhoAreYouArgs>();
            args.Age++;
            return new Result() { Message = JsonSerializer.Serialize(args) } ;
        }

        [TestItemParameter(typeof(InitializeArgs))]
        public IResult Initialize(JsonNode node)
        {
            
            Thread.Sleep(5000);
            var args = node.Deserialize<InitializeArgs>();
            var modFactory = this.Provider.GetRequiredService<IModuleFactory>();
            var modsinfo = this.Provider.GetAllModuleInfos();
            mod = modFactory.CreateModule(modsinfo["Dev1"]);
            return new Result() { Message = $"Dev1" };
        }

        [TestItemParameter(typeof(CloseArgs))]
        public IResult Close(JsonNode node)
        {
            var args = node.Deserialize<CloseArgs>();
            return new TestResult();
        }

        [TestItemParameter(typeof(DoArgs))]
        public IResult Do(JsonNode node)
        {
            var str=mod.Execute(CommandBase.Create("Method", "HELLO"));
            return new Result() { Message = str };
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

        public class Result : IResult
        {
            public DateTime TimeStamp { get; set; } = DateTime.Now;
            public ResultStatus Status { get; set; } = ResultStatus.Skip;

            public string Message { get; set; }
        }
    }
}




