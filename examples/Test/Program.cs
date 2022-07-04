// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using NetMQ;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

var hub = DevicesHub.Instance;
hub.Start();
hub.Devices["Dev1"].OnDataReady += ((result) => Console.WriteLine(result));
foreach (var item in hub.Devices)
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
    s.SendFrame(JsonConvert.SerializeObject(CommandBase.Create<TestBase.Data>
        ("Test",
        new TestBase.Data() { Date = DateTime.Now.ToString(), Number = new Random().NextDouble() })));
    //var a = hub.Devices["Dev1"].DataBus.GetData();
    //Console.WriteLine($"{a}");
}
Console.WriteLine("PRESS ANY KEY TO STOP");
Console.ReadKey();

hub.Stop();
Console.WriteLine("PRESS ANY KEY TO EXIT");
Console.ReadKey();
public class AnotherTest : DeviceBase
{
    public AnotherTest(IServiceProvider services, string configurationKey) : base(services, configurationKey)
    {
    }

    public override void Load(IConfigurationSection section)
    {
    }

    public override void LoadFromObject(object configuration)
    {
    }
}

public class TestBase : DeviceBase
{
    public TestBase(IServiceProvider services, string configurationKey) : base(services, configurationKey)
    {
    }

    public TestBase(object option, ITransceiver server, INotifier bus, IRecorder recorder) : base(option,server, bus, recorder)
    {
    }

    public class Data
    {
        public string Date { get; set; } = "";
        public double Number { get; set; } = 0.0;
    }

    [Prototype("Test", typeof(Command<Data>))]
    public string TAAest(Data a)
    {
        Instance.DataRecorder.Write(a);
        Instance.DataBus.Publish(a);
        return a.Date + "---" + a.Number.ToString();
    }

    [Prototype("Tes1t", typeof(Command<string>))]
    public string TA2Aest(string c)
    {
        return c;
    }

    public override void Load(IConfigurationSection section)
    {
    }
    public override void LoadFromObject(object configuration)
    {
    }
}