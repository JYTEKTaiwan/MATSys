// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;
using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Text.Json.Serialization;

var aa = JsonSerializer.Serialize(TestItemResult.Create(TestResultType.Pass, 0, null, null));
var b = JsonSerializer.Deserialize<TestItemResult>(aa);

Console.WriteLine("Hello, World!");

IHost host = Host.CreateDefaultBuilder().UseMATSys().Build();
host.RunAsync().Wait(1000); ;

var dev = host.Services.GetMATSysHandle();
dev.Modules["Dev1"].OnDataReady += (txt) =>
{
    //event is fired when internal notifier publishes data
};
dev.BeforeTestItem += (item) =>
{
    //event is fired before executing test item;    
};
dev.AfterTestItem += (item, res) =>
{
    //event is fired after executeing test item;
    Console.WriteLine($"{res}");
};
dev.RunTest(1);


Console.WriteLine("PRESS ANY KEY TO EXIT");

Console.ReadKey();
host.StopAsync();
public class TestDevice : ModuleBase
{

    public TestDevice(object configuration, ITransceiver server, INotifier bus, IRecorder recorder, string configurationKey = "") : base(configuration, server, bus, recorder, configurationKey)
    {
    }

    public override void Load(IConfigurationSection section)
    {
    }

    public override void Load(object configuration)
    {

    }

    public class Data
    {
        public string Date { get; set; } = "";
        public double Number { get; set; } = 0.0;
    }

    [MATSysCommand]
    public string Method(string c)
    {
        return c;
    }
}


