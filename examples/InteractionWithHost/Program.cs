// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;
using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Text.Json.Serialization;


IHost host = Host.CreateDefaultBuilder().UseMATSys().Build();
host.RunAsync().Wait(1000); ;


var dev = host.Services.GetRunner();

dev.AfterTestItemStops += (item, res) =>
{
    //event is fired after executeing test item;
    Console.WriteLine($"{res.ToJsonString()}");
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

    [MATSysCommand]
    public string Method(string c)
    {
        return c;
    }
}


