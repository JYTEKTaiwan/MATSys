// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;
using MATSys.Factories;
using MATSys.Modules;
using Microsoft.Extensions.Configuration;
using NetMQ;
using Newtonsoft.Json;
using System.Threading.Channels;

Console.WriteLine("Hello, World!");
var hub = ModuleHub.Instance;
hub.Start();
hub.Modules["Dev1"].OnDataReady += ((result) => Console.WriteLine(result));
foreach (var item in hub.Modules)
{
    Console.WriteLine($"==={item.Name}===");
    foreach (var cmd in item.PrintCommands())
    {
        Console.WriteLine(cmd);
    }
}
Console.ReadKey();
var sub = new NetMQ.Sockets.SubscriberSocket();
sub.Connect("inproc://127.0.0.1:12345");
sub.Subscribe("AA");
Task.Run(() =>
{
    while (true)
    {
        Console.WriteLine(sub.ReceiveMultipartStrings()[1]);
    }
});
var s = new NetMQ.Sockets.DealerSocket();
s.Connect("tcp://127.0.0.1:1234");
for (int i = 0; i < 10; i++)
{
    //Thread.Sleep();
    s.SendFrame(JsonConvert.SerializeObject(CommandBase.Create<TestDevice.Data>
        ("Test",
        new TestDevice.Data() { Date = DateTime.Now.ToString(), Number = new Random().NextDouble() })));
    //var a = hub.Devices["Dev1"].DataBus.GetData();
    //Console.WriteLine($"{a}");
}
Console.WriteLine("PRESS ANY KEY TO STOP");
Console.ReadKey();

hub.Stop();
Console.WriteLine("PRESS ANY KEY TO EXIT");
Console.ReadKey();

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
        Base.Recorder.Write(a);
        Base.Notifier.Publish(a);
        return a.Date + "---" + a.Number.ToString();
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
