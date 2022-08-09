// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;
using MATSys.Hosting;

Console.WriteLine("Hello, World!");

IHost host = Host.CreateDefaultBuilder().UseMATSys().Build();
host.RunAsync().Wait(1000);;

var dev = host.Services.GetMATSysHandle();
dev.Modules["Dev1"].OnDataReady+= IModule_OnDataReady;

void IModule_OnDataReady(string jsonString)
{
    Console.WriteLine(jsonString);
}

for (int i = 0; i < 10; i++)
{
   var response= dev.Execute("Dev1", CommandBase.Create<TestDevice.Data>
        ("Test",
        new TestDevice.Data() { Date = DateTime.Now.ToString(), Number = new Random().NextDouble() }));
    //Console.WriteLine(response);
}

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

    [MethodName("Test", typeof(Command<Data>))]
    public string Test(Data a)
    {
        var res = a.Date + "---" + a.Number.ToString();
        Base.Recorder.Write(a);
        Base.Notifier.Publish(a);
        return res;
    }

    [MethodName("Methhh4od", typeof(Command<string>))]
    public string Method(string c)
    {
        return c;
    }
}



public class CSVRecorderConfiguration
{
    public bool WaitForComplete { get; set; } = true;
    public int QueueSize { get; set; } = 2000;
    public int WaitForCompleteTimeout { get; set; } = 5000;
    public static CSVRecorderConfiguration Default = new CSVRecorderConfiguration();

    public BoundedChannelFullMode BoundedChannelFullMode { get; set; } = BoundedChannelFullMode.DropOldest;
}
