// See https://aka.ms/new-console-template for more information
using MATSys.Factories;
using MATSys.Plugins;
using SystemMonitorDEMO.Modules;
Console.WriteLine("Hello, World!");

var rec = RecorderFactory.CreateNew<CSVRecorder>(CSVRecorderConfiguration.Default);
var notConfig = new NetMQNotifierConfiguration() { Address = "127.0.0.1:5000", Topic = "AA" };
var not = NotifierFactory.CreateNew<NetMQNotifier>(notConfig);


var mon = ModuleFactory.CreateNew(typeof(SystemMonitor), null, recorder: rec, notifier: not);
//var mon = new SystemMonitor(null, null, null, new TextRecorder()) as IModule;
mon.StartService(new CancellationToken());
mon.Notifier.OnNotify += (string dataInJson) => Console.WriteLine(dataInJson);
var id = mon.Execute("ID=");
var response = mon.Execute("Machine=");

mon.Execute("StartMonitor=");

var cts = new CancellationTokenSource();
Task.Run(() =>
{
    while (!cts.IsCancellationRequested)
    {
        var data = mon.Execute("GetLatestData=");
        Console.Write("\r" + data);
    }
});


Console.ReadLine();
cts.Cancel();
mon.Execute("StopMonitor=");
mon.StopService();