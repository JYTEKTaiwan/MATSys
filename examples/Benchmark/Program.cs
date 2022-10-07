// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;
using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

IHost host = Host.CreateDefaultBuilder().UseMATSys().Build();
host.RunAsync().Wait(1000); ;

var dev = host.Services.GetMATSysHandle();
var iteration = 1000000;
Console.WriteLine($"Send same command for {iteration} times");

dev.ExecuteCommand("Dev1", $"StringMethod=\"tt\"");
Stopwatch stopwatch = new Stopwatch();
stopwatch.Restart();
for (int i = 0; i < iteration; i++)
{
    dev.ExecuteCommand("Dev1", $"StringMethod=\"{i}\"");
}
stopwatch.Stop();
Console.WriteLine($"Total {stopwatch.Elapsed.TotalSeconds} seconds (Average:{stopwatch.Elapsed.TotalMilliseconds / iteration} ms)");


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
    public string Test(Data a)
    {
        var res = a.Date + "---" + a.Number.ToString();
        Base.Recorder.Write(a);
        Base.Notifier.Publish(a);
        return res;
    }

    [MATSysCommand("StringMethod")]
    public string Method(string c)
    {
        Base.Notifier.Publish(c);

        return c;
    }
}


