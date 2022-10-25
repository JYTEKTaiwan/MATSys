// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;
using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, World!");

IHost host = Host.CreateDefaultBuilder().UseMATSys().Build();
host.RunAsync().Wait(1000); ;

var dev = host.Services.GetMATSysHandle();

dev.Modules["Dev1"].OnDataReady += IModule_OnDataReady;
dev.OnReadyToExecute += (cmd) => { Console.WriteLine($"{cmd.ModuleName}*{cmd.Command}"); };
dev.OnExecuteComplete += (item, res) => { Console.WriteLine($"{res}"); };
dev.RunTest(3);

void IModule_OnDataReady(string jsonString)
{
    Console.WriteLine(jsonString);
}

//for (int i = 0; i < 10; i++)
//{
//   var response= dev.Execute("Dev1", CommandBase.Create<TestDevice.Data>
//        ("Test",
//        new TestDevice.Data() { Date = DateTime.Now.ToString(), Number = new Random().NextDouble() }));
//    //Console.WriteLine(response);
//}

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


