using MATSys;
using MATSys.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Linq;
using System.Xml.Serialization;
using CsvHelper.Configuration.Attributes;


namespace MATSys.Automation;
public class TestRunner
{
    private IServiceCollection services = new ServiceCollection();
    private IServiceScope scopedServices;
    private TestPackage output;
    private TestSummary summary = new TestSummary();
    private CancellationTokenSource cts;

    private IReportWriter<ReportContext> writer = new ReportWriter<ReportContext>();
    public TestPackage TestPackage { get; private set; }
    public UUT UUT { get; set; }
    public Operator Operator { get; set; }

    public event EventHandler? PackageLoading;
    public event EventHandler<Exception>? PackageLoadedException;
    public event EventHandler? PackageLoaded;

    public event EventHandler? RunnerExecutionOnstart;
    public event EventHandler<TestItem>? TestItemExecutionOnstart;
    public event EventHandler<int>? TestItemRetryOnstart;
    public event EventHandler<TestResult>? AfterTestItemCompleted;
    public event EventHandler<TestPackage>? AfterRunnerExecutionCompleted;
    public event EventHandler<TestSummary>? TestSummaryGenerated;
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
            // create the linked cancelation token 
            cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            // initialize the IReportWriter by using UUT's name as prefix
            writer.Initialize(UUT.Name);
            // fire the event 
            RunnerExecutionOnstart?.Invoke(this, null);
            // cleanup the summary 
            summary.Init();
            // duplicate the test package for output usage
            output = TestPackage;
            // iterate the test items
            foreach (var item in output.Items)
            {
                // if the token is cancelled, exit the execution process
                if (cts.IsCancellationRequested) break;
                // fire the event
                TestItemExecutionOnstart?.Invoke(this, item);
                // execute the test item
                await ExecuteAsync(item, cts.Token);
                // update the result to IReportWriter
                writer.WriteReport(new ReportContext(UUT, Operator, item, output));
                // update the summary counting
                summary.Update(item);
                // fire the event
                AfterTestItemCompleted?.Invoke(this, item.Result.Value);
            }
            // flush and clean up the IReportWriter
            writer.Flush();
            writer.Close();
            // fire the event
            AfterRunnerExecutionCompleted?.Invoke(this, output);
            TestSummaryGenerated?.Invoke(this, summary);
            return output;
        });

    }
    private async Task ExecuteAsync(TestItem item, CancellationToken token = default)
    {
        await Task.Run(async () =>
        {
            if (item.Skip)
            {
                // if the test item marked as skip, return the test result as Skip
                item.Result = TestResult.Create(TestResultType.Skip, null);
            }
            else
            {
                var testObject = GetModule(item.PluginID);
                var cmd = ParseCommand(item);
                int iteration = item.Retry < 1 ? 0 : item.Retry;
                for (int i = 0; i <= iteration; i++)
                {
                    if (token.IsCancellationRequested) { item.Result = TestResult.Create(TestResultType.Ignore, null); break; }
                    TestItemRetryOnstart?.Invoke(this, i);
                    testObject.Execute(cmd, out var result);
                    TestResult res = (TestResult)result;
                    res.IterationCount = i;
                    item.Result = res;
                    if (((TestResult)result).Result == TestResultType.Pass) { break; }
                }

            }
        });

    }
    private IModule GetModule(string key)
    {
        return scopedServices.ServiceProvider.GetRequiredKeyedService<IModule>(key);
    }

    private string ParseCommand(TestItem item)
    => string.IsNullOrEmpty(item.Method) ?
                $"{{\"{item.Name}\":[{item.Parameters.ToString()},{item.Conditions.ToString()}]}}"
                : $"{{\"{item.Method}\":[{item.Parameters.ToString()},{item.Conditions.ToString()}]}}";

    private void ValidateTestPackageInfo(TestPackage package)
    {
        if (package.Plugins.Select(x => x.Alias).Distinct().Count() != package.Plugins.Count())
            throw new PackageLoadingException("Duplicated plugin IDs");


        foreach (var plugin in package.Plugins)
        {
            if (string.IsNullOrEmpty(plugin.Alias)) throw new PackageLoadingException("Plugin ID cannot be empty");

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
            if (!ids.Any(x => x == id)) throw new PackageLoadingException($"Wrong id for test item #{item.Order}:{item.Name}");

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
        var plugin = package.Plugins[item.PluginID];
        AssemblyPath = plugin.Path;
        TypeInfo = plugin.QualifiedName;
        TestResult = item.Result.Value;
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

