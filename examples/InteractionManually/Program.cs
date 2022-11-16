using MATSys;
using MATSys.Commands;
using MATSys.Factories;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using NetMQ;
using NetMQ.Sockets;
using NLog.Extensions.Logging;

var opt = new NetMQTransceiverConfiguration() { LocalIP = "tcp://127.0.0.1", Port = 1234 };
var n = TransceiverFactory.CreateNew<NetMQTransceiver>(opt);

var a = ModuleFactory.CreateNew<TestModule>(null, n, null, null, "TEST");
a.StartService(new CancellationToken());

//var response = a.Execute(CommandBase.Create("Method", "HELLO"));

var client = new DealerSocket();
client.Connect("tcp://127.0.0.1:1234");
client.SendFrame("{\"Method\":[\"WORLD\"]}");
var response = client.ReceiveFrameString();

//var response = a.Execute(CommandBase.Create("Method", "TEST"));

//var response = a.Execute("{\"Method\":[\"Hello\"]}");
Console.WriteLine(response);

a.StopService();


Console.ReadLine();


public class TestModule:ModuleBase
{
    public TestModule(object? configuration, ITransceiver? transceiver, INotifier? notifier, IRecorder? recorder, string aliasName = "") : base(configuration, transceiver, notifier, recorder, aliasName)
    {

    }

    [MATSysCommand]
    public string Method(string c)
    {
        return $"{c} from TestModule";
    }

}
