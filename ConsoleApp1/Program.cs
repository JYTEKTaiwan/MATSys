// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Text.Json.Nodes;

var host=Host.CreateDefaultBuilder().ConfigureServices(s =>
s.AddSingleton<ITestPackage, MyTestPackage>()).Build();
host.StartAsync().Wait(500);
var tp=host.Services.GetRequiredService<ITestPackage>();
var res=tp.Execute("WhoAreYou", JsonSerializer.SerializeToNode(new MyTestPackage.WhoAreYouArgs()));
Console.WriteLine(res.ToJsonString());
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