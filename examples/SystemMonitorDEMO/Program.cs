// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Factories;
using SystemMonitorDEMO.Modules;
Console.WriteLine("Hello, World!");


var mon=ModuleFactory.CreateNew(typeof(SystemMonitor),null);
//var mon = new SystemMonitor(null, null, null, new TextRecorder()) as IModule;
mon.StartService(new CancellationToken());
mon.Notifier.OnNotify += Notifier_OnNotify;

void Notifier_OnNotify(string dataInJson)
{
    Console.WriteLine(dataInJson);
}

var id =  mon.ExecuteAsync("ID=");

var response =mon.ExecuteAsync("Machine=");

Console.WriteLine(DateTime.Now);

await Task.Delay(1000);

mon.StopService();