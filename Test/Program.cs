using MATSys;
using MATSys.Automation;
using MATSys.Commands;
using CsvHelper;
using System.Text.Json;

var tp = new TestPackage();
tp.Plugins.Clear();
tp.Plugins.Add(new PluginInfo()
{
    Alias = "hi",
    Path = typeof(Device).Assembly.Location,
    QualifiedName = typeof(Device).AssemblyQualifiedName
});
tp.Plugins.Add(new PluginInfo()
{
    Alias = "hi2",
    Path = typeof(Device2).Assembly.Location,
    QualifiedName = typeof(Device2).AssemblyQualifiedName
});

tp.Items = new TestItemCollection(Debug_GetTestItems());
tp.Export("test.xml");
var tp_fromFile = TestPackage.Load("test.xml");

var str = tp_fromFile.Items[0].Parameters.ToString();

var obj = JsonSerializer.Deserialize<ParameterCollection>(str);

var runner = new TestRunner();

runner.UUT = new UUT()
{
    ID = "testUUT001",
    Name = "TestUUT",
    PN = "123-45677",
    SN = "ACBEDGE",
    Version = new Version("1.2.4.1204")
};
runner.Operator = new Operator() { Employee = "Way", Group = "JYTEK" };

runner.PackageLoaded += (sender, e) =>                  Console.WriteLine($"[{DateTime.Now}] Package loaded");
runner.RunnerExecutionOnstart += (sender, e) =>         Console.WriteLine($"[{DateTime.Now}] Runner starts");
runner.TestItemExecutionOnstart += (sender, e) =>       Console.WriteLine($"[{DateTime.Now}] Executing - {e.Name}");
runner.TestItemRetryOnstart += (sender, e) =>           Console.WriteLine($"[{DateTime.Now}] Retry#{e}");
runner.AfterTestItemCompleted += (sender, e) =>         Console.WriteLine($"[{e.TimeStamp}] {e.Result}@{e.Value}");
runner.AfterRunnerExecutionCompleted += (sender, e) =>  Console.WriteLine($"[{DateTime.Now}] Runner completed");
runner.TestSummaryGenerated += (sender, e) =>           Console.WriteLine($"[{DateTime.Now}] Summary: {e}");


runner.Load(tp_fromFile);
var cts = new CancellationTokenSource();
var result =await  runner.RunAsync(cts.Token);

//result.Export("result.xml");
runner.Stop();

Console.ReadKey();


static IEnumerable<TestItem> Debug_GetTestItems()
{
    yield return new TestItem()
    {
        Name = "LargerThan",
        Order = 1,
        Retry = 6,
        Skip = false,
        Description = "",
        PluginID = "hi",
        Parameters = ParameterCollection.Create(("Number", 10)),
        Conditions = ParameterCollection.Create(("Limit", 3.6))
    };
    yield return new TestItem()
    {
        Name = "SmallerThan",
        Order = 2,
        Retry = 6,
        Skip = false,
        Description = "",
        PluginID = "hi2",
        Parameters = ParameterCollection.Create(("Number", 6.5)),
        Conditions = ParameterCollection.Create(("Limit", 3))
    };
    yield return new TestItem()
    {
        Name = "SmallerThan",
        Order = 2,
        Retry = 6,
        Skip = true,
        Description = "",
        PluginID = "hi2",
        Parameters = ParameterCollection.Create(("Number", 10)),
        Conditions = ParameterCollection.Create(("Limit", 3))
    };
}

public class Device : ModuleBase
{
    public override object Configuration { get; set; }

    [MATSysCommand]
    public TestResult LargerThan(Input input, Compare compare)
    {
        var v = Random.Shared.Next(0, (int)compare.Limit + 2);
        var res = v > compare.Limit;
        var tr = TestResult.Create(res ? TestResultType.Pass : TestResultType.Fail, v.ToString());
        tr.Message = "LagerThan";
        return tr;
    }
}
public record Input(decimal Number);
public record Compare(decimal Limit);

public record HighLow(decimal highLimit, decimal lowLimit, string hi);

public class Device2 : ModuleBase
{
    public override object Configuration { get; set; }

    [MATSysCommand]
    public TestResult SmallerThan(Input input, Compare compare)
    {
        var v = Random.Shared.Next(0, (int)compare.Limit + 2);
        var res = v < compare.Limit;
        var tr = TestResult.Create(res ? TestResultType.Pass : TestResultType.Fail, v.ToString());
        tr.Message = "LagerThan";
        return tr;
    }
    [MATSysCommand]

    public TestResult Between(Input input, HighLow compare)
    {
        var v = Random.Shared.Next((int)compare.lowLimit - 2, (int)compare.highLimit + 2);
        var res = v < compare.highLimit && v > compare.lowLimit;
        var tr = TestResult.Create(res ? TestResultType.Pass : TestResultType.Fail, v.ToString());
        tr.Message = "LagerThan";
        return tr;
    }

}
