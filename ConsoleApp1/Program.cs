// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Hosting;
using MATSys.Hosting.Scripting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

var host = Host.CreateDefaultBuilder().UseMATSys().Build();
host.StartAsync().Wait(500);
var runner = host.GetMATSysRunner();
var a=runner.RunTest();
foreach (var item in a)
{
    Console.WriteLine(item.ToJsonString());
}
Console.ReadLine();
host.StopAsync();

internal class MyTestPackage : TestPackageBase
{

    [TestItemParameter(typeof(WhoAreYouArgs))]
    public JsonNode WhoAreYou(JsonNode node)
    {
        var args = node.Deserialize<WhoAreYouArgs>();
        args.Name = "Way";
        return JsonSerializer.SerializeToNode(args) ;
    }

    internal class WhoAreYouArgs
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}