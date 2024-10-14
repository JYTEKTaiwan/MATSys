using MATSys;
using MATSys.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Linq;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;


namespace MATSys.Automation;
public class TestRunner
{
    private IServiceCollection services = new ServiceCollection();
    private IServiceScope scopedServices;
    private TestPackage output;
    private TestSummary summary = new TestSummary();
    private CancellationTokenSource cts;

    private IReportWriter<ReportContext> writer=new ReportWriter<ReportContext>();
    public TestPackage TestPackage { get; private set; }
    public UUT UUT { get; set; }
    public Operator Operator { get; set; }

    public event EventHandler PackageLoading;
    public event EventHandler<Exception> PackageLoadedException;
    public event EventHandler PackageLoaded;

    public event EventHandler RunnerExecutionOnstart;
    public event EventHandler<TestItem> TestItemExecutionOnstart;
    public event EventHandler<int> TestItemRetryOnstart;
    public event EventHandler<TestResult> AfterTestItemCompleted;
    public event EventHandler<TestPackage> AfterRunnerExecutionCompleted;
    public event EventHandler<TestSummary> TestSummaryGenerated;
    public void Load(TestPackage package)
    {
        PackageLoading?.Invoke(this, EventArgs.Empty);
        try
        {
            TestPackage = package;

            ValidateTestPackageInfo(package);

            ValidateTestItems(package);

            ConfigureServices(package);
        }
        catch (Exception ex)
        {
            PackageLoadedException.Invoke(this, ex);
            throw ex;
        }


        PackageLoaded?.Invoke(this, EventArgs.Empty);
    }

    public TestPackage Run()
    {
        cts = new CancellationTokenSource();
        return RunAsync(cts.Token).Result;
    }
    public void Stop()
    {
        cts?.Cancel();
    }
    public async Task<TestPackage> RunAsync(CancellationToken token = default)
    {
        return await Task.Run(async () =>
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            writer.Initialize(UUT.Name);
            RunnerExecutionOnstart?.Invoke(this, null);
            summary.Init();
            output = TestPackage;
            foreach (var item in output.Items)
            {
                if (cts.IsCancellationRequested) break;
                TestItemExecutionOnstart?.Invoke(this, item);
                if (!item.Skip)
                {
                    await ExecuteAsync(item, cts.Token);
                }
                else
                {
                    item.Result = TestResult.Create(TestResultType.Skip, null);
                }
                writer.WriteReport(new ReportContext(UUT,Operator,item,output));
                summary.Update(item);
                AfterTestItemCompleted?.Invoke(this, item.Result);
            }
            writer.Flush();
            writer.Close();
            AfterRunnerExecutionCompleted?.Invoke(this, output);
            TestSummaryGenerated?.Invoke(this, summary);
            return output;
        });

    }
    private async Task ExecuteAsync(TestItem item, CancellationToken token = default)
    {
        await Task.Run(async () =>
        {
            var key = item.PluginID;
            var testObject = scopedServices.ServiceProvider.GetRequiredKeyedService<IModule>(key);
            var cmd = string.IsNullOrEmpty(item.Method) ?
            $"{{\"{item.Name}\":[{item.Parameters.ToString()},{item.Conditions.ToString()}]}}"
            : $"{{\"{item.Method}\":[{item.Parameters.ToString()},{item.Conditions.ToString()}]}}";
            int cnt = 0;
            for (int i = 0; i < item.Retry; i++)
            {
                if (token.IsCancellationRequested) break;
                cnt++;
                TestItemRetryOnstart?.Invoke(this, cnt);
                testObject.Execute(cmd, out var result);
                item.Result = (TestResult)result;
                item.Result.IterationCount = cnt;
                if (((TestResult)result).Result == TestResultType.Pass) { break; }
            }
        });

    }

    private void ValidateTestPackageInfo(TestPackage package)
    {
        if (package.Plugins.Select(x => x.Alias).Distinct().Count()!=package.Plugins.Count())
            throw new PackageLoadingException("Duplicated plugin IDs");


        foreach (var plugin in package.Plugins)
        {
            if(string.IsNullOrEmpty(plugin.Alias)) throw new PackageLoadingException("Plugin ID cannot be empty");

            //check if dll path in the plugins information exists
            if (!File.Exists(plugin.Path)) throw new PackageLoadingException("Invalid plugin path");

            //check if type in the plugins information exists
            var typeName = plugin.QualifiedName.Split(',').First();
            var t = TypeParser.SearchType(typeName, plugin.Path);
            if (t == null) throw new PackageLoadingException("Invalid plugin type");
        }
        
    }
    private void ValidateTestItems(TestPackage package)
    {
        var ids = package.Items.Select(x => x.PluginID);
        foreach (var item in package.Items)
        {
            //check if id of testitem is not existed
            var id = item.PluginID;
            if (string.IsNullOrEmpty(id)) throw new PackageLoadingException($"Id of test item #{item.Order}:{item.Name} cannot be null");
            if (!ids.Any(x=>x==id)) throw new PackageLoadingException($"Wrong id for test item #{item.Order}:{item.Name}");
                        
        }
    }

    private void ConfigureServices(TestPackage package)
    {
        scopedServices?.Dispose();
        services.Clear();
        //add plugins library info
        foreach (var item in package.Plugins)
        {
            var typeName = item.QualifiedName.Split(',').First();
            var t = TypeParser.SearchType(typeName, item.Path);
            if (string.IsNullOrEmpty(item.Alias)) break;
            services.AddKeyedScoped(item.Alias, (s, k) =>
            {
                var obj = (IModule)Activator.CreateInstance(t);
                obj.Alias = k.ToString();
                obj.SetProvider(s);
                return obj;
            });
        }
        scopedServices = services.BuildServiceProvider().CreateScope();
        writer.Flush();
        writer.Close();
    }
    ~TestRunner()
    {
        services.Clear();
        scopedServices.Dispose();
    }
}


public interface IReportWriter<T>
{
    void Initialize(params object[] args);
    void WriteReport(T report);

    void Flush();

    void Close();

}
public class ReportWriter<T> : IReportWriter<T>
{
    private StreamWriter sw;
    private CsvHelper.CsvWriter writer;
    public void Close()
    {
        writer?.Dispose();
        sw?.Close();
    }

    public void Flush()
    {
        writer?.Flush();
        sw?.Flush();
    }

    public void Initialize(params object[] args)
    {
        if (args.Length != 1) throw new ArgumentOutOfRangeException();
        var prefix = args[0].ToString();
        var path = Path.GetFullPath($"{prefix}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv");
        sw = new StreamWriter(path);
        writer = new CsvHelper.CsvWriter(sw, System.Globalization.CultureInfo.InvariantCulture);
        writer.WriteHeader(typeof(T));
        writer.NextRecord();
    }

    public void WriteReport(T report)
    {
        writer?.WriteRecord(report);
        writer.NextRecord();
    }
}
public class UUT
{
    [Index(0)]
    public string Name { get; set; } = "";

    [Index(1)]
    public string ID { get; set; } = "";
    [Index(2)]
    public string PN { get; set; } = "";
    [Index(3)]
    public string SN { get; set; } = "";
    [Index(4)]
    public int Major => Version.Major;
    [Index(5)]
    public int Minor => Version.Minor;
    [Index(6)]
    public int Build => Version.Build;
    [Index(7)]
    public int Revision => Version.Revision;

    [Ignore]
    public Version Version { get; set; } = new Version("0.0.0.0");

}

public class Operator
{
    [Index(8)]
    public string Employee { get; set; } = "Employee";
    [Index(9)]
    public string Group { get; set; } = "Engineering";

}
public struct ReportContext
{
    public ReportContext(
        UUT uut, Operator op, TestItem item, TestPackage package)
    {
        UUT = uut;
        Operator = op;
        TestItem = item.Name;
        Order = item.Order;
        Skip = item.Skip;
        Retry = item.Retry;
        Arguments = item.Arguments;
        Description = item.Description;
        Method = item.Method;
        Parameters = item.Parameters.ToString();
        Conditions = item.Conditions.ToString();
        var plugin=package.Plugins[item.PluginID];
        AssemblyPath = plugin.Path;
        TypeInfo = plugin.QualifiedName;
        TestResult = item.Result;
    }
    public UUT UUT { get; set; }

    
    public Operator Operator { get; set; }
    public string TestItem { get; set; } = "";
    public int Order { get; set; }
    public bool Skip { get; set; }
    public int Retry { get; set; }      
    public string Method { get; set; } = "";
    public string Parameters { get; set; } = "";
    public string Conditions { get; set; } = "";
    public string Arguments { get; set; } = "";
    public string Description { get; set; } = "";
    public TestResult TestResult { get; set; }
    public string TypeInfo { get; set; } = "";
    public string AssemblyPath { get; set; } = "";

}

