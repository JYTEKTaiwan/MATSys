using MATSys;
using MATSys.Commands;
using MATSys.Factories;

using MATSys.Plugins;
using NetMQ;
using NetMQ.Sockets;


var a = ModuleFactory.CreateNew<TestModule>(null);

var response = a.Execute(CommandBase.Create("Method", "HELLO"));
Console.WriteLine(response);



Console.ReadLine();


public class TestModule : ModuleBase
{

    [MATSysCommand]
    public string Method(string c)
    {
        return $"{c} from TestModule";
    }

}
