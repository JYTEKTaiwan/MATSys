// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;


    var tp = new TestPackage();
tp.Modules.Add(new ModuleInfo()
{
    Name = "Device", 
    Path = typeof(Device).Assembly.Location,
    QualifiedName = typeof(Device).AssemblyQualifiedName,
    Notifier= new PluginInfo()
    {
        Path = typeof(Device).Assembly.Location,
        QualifiedName = typeof(Device).AssemblyQualifiedName,
    }

}

);

tp.Items = Debug_GetTestItems().ToList();
tp.Export("test.xml");

var tp_fromFile = TestPackage.Load("test.xml");
var runner = new TestRunner();

runner.PackageLoadedException += (sender, e) => Console.WriteLine(e.Message);
runner.RunnerExecutionOnstart += (sender, e) => Console.WriteLine("Runner starts");
runner.TestItemExecutionOnstart += (sender, e) => Console.WriteLine("Item starts");
runner.TestItemRetryOnstart += (sender, e) => Console.WriteLine($"Retry#{e}");
runner.AfterTestItemCompleted += (sender, e) => Console.WriteLine($"Item completed: {e}");
runner.AfterRunnerExecutionCompleted += (sender, e) => Console.WriteLine("Runner completed");
runner.TestSummaeyGenerated += (sender, e) => Console.Write(e);
runner.Load(tp_fromFile);
var cts= new CancellationTokenSource();
var result=runner.Run();
result.Export("result.xml");
runner.Stop();

Console.ReadKey();


static IEnumerable<TestItem> Debug_GetTestItems()
{
    for (int i = 0; i < 5; i++)
    {
        yield return new TestItem()
        {
            Name = (i * 11).ToString(),
            Order = i,
            Retry = 6,
            Skip = i<1,
            Description = (i - 100).ToString(),
            Worker = new Worker()
            {
                Alias = "Device",
                MethodName = "LargerThan",
                Parameters = ParameterCollection.Create(("Number", i)),
                Conditions = ParameterCollection.Create(("Limit", 3))
            },

        };
    }
}

public class Device:ModuleBase
{
    public override object Configuration { get; set; }

    [MATSysCommand]
    public TestResult LargerThan(Input input, Compare compare)
    { 
        var v = Random.Shared.Next(0, (int)compare.Limit + 2);
        var res =  v> compare.Limit;
        var tr= TestResult.Create(res? TestResultType.Pass: TestResultType.Fail,v.ToString()) ;
        tr.Message = "LagerThan";
        return tr;
    }
}
public record Input(decimal Number);
public record Compare(decimal Limit);