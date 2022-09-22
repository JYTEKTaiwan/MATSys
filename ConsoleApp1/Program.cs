using MATSys;
using MATSys.Commands;
using MATSys.Factories;
using MATSys.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Security.Policy;



foreach (var item in MATSysHelper.ListAllPlugins())
{
    Console.WriteLine($"{item.Category}\t{item.Type}\t{item.AssemblyPath}");
}
foreach (var item in MATSysHelper.ListAllModules())
{
    Console.WriteLine($"{item.Category}\t{item.Type}\t{item.AssemblyPath}");
    foreach (var cmds in ModuleHelper.ShowSupportedCommands(item.Type))
    {
        Console.WriteLine(cmds);
    }

}

Console.ReadKey();

public class Test : ModuleBase
{
    public Test(object? configuration, ITransceiver? transceiver, INotifier? notifier, IRecorder? recorder, string aliasName = "") : base(configuration, transceiver, notifier, recorder, aliasName)
    {
    }

    public override void Load(Microsoft.Extensions.Configuration.IConfigurationSection section)
    {
        throw new NotImplementedException();
    }

    public override void Load(object configuration)
    {
        throw new NotImplementedException();
    }

    [MATSysCommand]
    public void Test1()
    {

    }

    [MATSysCommand("Another")]
    public void Test2(int a,double b, string c, DateTime d,bool e)
    {

    }

}
