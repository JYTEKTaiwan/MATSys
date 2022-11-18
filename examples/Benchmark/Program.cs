// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;
using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Drawing.Text;
using System.Runtime.InteropServices.ObjectiveC;

IHost host = Host.CreateDefaultBuilder().ConfigureLogging(log=>log.ClearProviders()).UseMATSys().Build();

host.RunAsync().Wait(1000); ;

var dev = host.Services.GetRunner();
var iteration = 1000000;
Stopwatch stopwatch = new Stopwatch();

Console.WriteLine($"Send command (no arguments) for {iteration} times");
var cmd = CommandBase.Create("NoArgument").Serialize();
Console.WriteLine($"Command size={cmd.Length} bytes");
dev.Execute("Dev1", cmd);
stopwatch.Restart();
for (int i = 0; i < iteration; i++)
{
    dev.Execute("Dev1", cmd);
}
stopwatch.Stop();
Console.WriteLine($"Total {stopwatch.Elapsed.TotalSeconds} seconds (Average:{stopwatch.Elapsed.TotalMilliseconds / iteration} ms)");

Console.WriteLine();

Console.WriteLine($"Send command (1 arguments) for {iteration} times");
cmd = CommandBase.Create("OneArgument","HI").Serialize();
Console.WriteLine($"Command size={cmd.Length} bytes");
dev.Execute("Dev1", cmd);
stopwatch.Restart();
for (int i = 0; i < iteration; i++)
{
    dev.Execute("Dev1", cmd);
}
stopwatch.Stop();
Console.WriteLine($"Total {stopwatch.Elapsed.TotalSeconds} seconds (Average:{stopwatch.Elapsed.TotalMilliseconds / iteration} ms)");

Console.WriteLine();

Console.WriteLine($"Send command (7 arguments) for {iteration} times");
cmd = CommandBase.Create("SevenArgument", 1,1.0,"HI",true,DateTime.Now,new Object(),(decimal)10).Serialize();
Console.WriteLine($"Command size={cmd.Length} bytes");
dev.Execute("Dev1", cmd);
stopwatch.Restart();
for (int i = 0; i < iteration; i++)
{
    dev.Execute("Dev1", cmd);
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

    public class Data
    {
        public string Date { get; set; } = "";
        public double Number { get; set; } = 0.0;
    }

    [MATSysCommand]
    public string SevenArgument(int a,double b, string c, bool d, DateTime e, object f, decimal g)
    {
        return "";
    }

    [MATSysCommand]
    public string OneArgument(string a)
    {
        return "";
    }

    [MATSysCommand]
    public string NoArgument()
    {
        return "";
    }
}


