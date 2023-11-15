// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MATSys.Commands;
using MATSys.Utilities;

BenchmarkRunner.Run<CommandBenchmark>();


[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net70)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net60)]
public class TypeParserBenchmark
{

    const string path = @"D:\codes\MATSys\plugins\MATSys.Plugins.NetMQTransceiver\bin\Debug\net6.0\MATSys.Plugins.NetMQTransceiver.dll";
    [Benchmark]
    public void ExistedType()
    {
        TypeParser.SearchType("MATSys.Utilities.TypeParser", "");
    }
    [Benchmark]
    public void ExternalType()
    {

        TypeParser.SearchType("MATSys.Plugins.NetMQTransceiver", path);
    }
}

[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net70)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net60)]
public class CommandBenchmark
{
    private ICommand cmd0 = CommandBase.Create("NoArgument");
    private ICommand cmd1 = CommandBase.Create("OneArgument", "HI");
    private ICommand cmd2 = CommandBase.Create("TwoArgument", 1, 1.0);
    private ICommand cmd3 = CommandBase.Create("ThreeArgument", 1, 1.0, "HI");
    private ICommand cmd4 = CommandBase.Create("FourArgument", 1, 1.0, "HI", true);
    private ICommand cmd5 = CommandBase.Create("FiveArgument", 1, 1.0, "HI", true, DateTime.Now);
    private ICommand cmd6 = CommandBase.Create("SixArgument", 1, 1.0, "HI", true, DateTime.Now, new Object());
    private ICommand cmd7 = CommandBase.Create("SevenArgument", 1, 1.0, "HI", true, DateTime.Now, new Object(), (decimal)10);
    private string cmd0_str = CommandBase.Create("NoArgument").Serialize();
    private string cmd1_str = CommandBase.Create("OneArgument", "HI").Serialize();
    private string cmd2_str = CommandBase.Create("TwoArgument", 1, 1.0).Serialize();
    private string cmd3_str = CommandBase.Create("ThreeArgument", 1, 1.0, "HI").Serialize();
    private string cmd4_str = CommandBase.Create("FourArgument", 1, 1.0, "HI", true).Serialize();
    private string cmd5_str = CommandBase.Create("FiveArgument", 1, 1.0, "HI", true, DateTime.Now).Serialize();
    private string cmd6_str = CommandBase.Create("SixArgument", 1, 1.0, "HI", true, DateTime.Now, new Object()).Serialize();
    private string cmd7_str = CommandBase.Create("SevenArgument", 1, 1.0, "HI", true, DateTime.Now, new Object(), (decimal)10).Serialize();
    [Benchmark]
    public void Arg0_Serialize()
    {
        cmd0.Serialize();
    }
    [Benchmark]
    public void Arg0_Deserialize()
    {
        CommandBase.Deserialize(cmd0_str,cmd0.GetType());
    }
    [Benchmark]
    public void Arg7_Serialize()
    {
        cmd7.Serialize();
    }
    [Benchmark]
    public void Arg7_Deserialize()
    {
        CommandBase.Deserialize(cmd7_str,cmd7.GetType());
    }

}