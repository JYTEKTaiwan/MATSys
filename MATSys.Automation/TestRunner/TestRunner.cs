using MATSys;
using MATSys.Utilities;
using Microsoft.Extensions.DependencyInjection;

public class TestRunner
{
    private IServiceCollection services = new ServiceCollection();
    private IServiceScope scopedServices;
    private TestPackage output;
    private TestSummary summary = new TestSummary();
    private CancellationTokenSource cts;
    public TestPackage TestPackage { get; private set; }

    public event EventHandler PackageLoading;
    public event EventHandler<Exception> PackageLoadedException;
    public event EventHandler PackageLoaded;

    public event EventHandler RunnerExecutionOnstart;
    public event EventHandler<TestItem> TestItemExecutionOnstart;
    public event EventHandler<uint> TestItemRetryOnstart;
    public event EventHandler<TestResult> AfterTestItemCompleted;
    public event EventHandler<TestPackage> AfterRunnerExecutionCompleted;
    public event EventHandler<TestSummary> TestSummaeyGenerated;
    public void Load(TestPackage package)
    {
        PackageLoading?.Invoke(this, EventArgs.Empty);
        try
        {
            TestPackage = package;

            ValidatePlugins(package);

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
        cts.Cancel();
    }
    public async Task<TestPackage> RunAsync(CancellationToken token = default)
    {
        return await Task.Run(async () =>
        {
            RunnerExecutionOnstart?.Invoke(this, null);
            summary.Init();
            output = TestPackage;
            foreach (var item in output.Items)
            {
                TestItemExecutionOnstart?.Invoke(this, item);
                if (!item.Skip && !token.IsCancellationRequested)
                {
                    await ExecuteAsync(item,token);
                }
                else
                {
                    item.Result = TestResult.Create(TestResultType.Skip,null);
                }
                summary.Update(item);
                AfterTestItemCompleted?.Invoke(this, item.Result);                
            }
            AfterRunnerExecutionCompleted?.Invoke(this, output);
            TestSummaeyGenerated?.Invoke(this, summary);
            return output;
        });

    }
    private async Task ExecuteAsync(TestItem item, CancellationToken token = default)
    {
        await Task.Run(async () =>
        {
            uint cnt = 0;
            for (int i = 0; i < item.Retry; i++)
            {
                if (token.IsCancellationRequested) break;
                cnt++;
                TestItemRetryOnstart?.Invoke(this, cnt);
                var mod = scopedServices.ServiceProvider.GetKeyedService<IModule>(item.Worker.Alias);
                mod.Execute(item.Worker.ToString(), out var result);                
                item.Result = (TestResult)result;
                item.Result.IterationCount = cnt;                
                if (((TestResult)result).Result == TestResultType.Pass) { break; }
            }
        });

    }

    private void Execute(TestItem item)
    {
        uint cnt = 0;

        for (int i = 0; i < item.Retry; i++)
        {
            cnt++;
            TestItemRetryOnstart?.Invoke(this, cnt);
            var mod = scopedServices.ServiceProvider.GetKeyedService<IModule>(item.Worker.Alias);
            mod.Execute(item.Worker.ToString(), out var result);
            item.Result = (TestResult)result;
            item.Result.IterationCount = cnt;
            if (((TestResult)result).Result == TestResultType.Pass) { break; }
        }
    }
    private void ValidatePlugins(TestPackage package)
    {
        foreach (var plugin in package.Modules)
        {
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
        if (package.Items.Count != package.Items.Select(x => x.Order).Distinct().Count())
            throw new PackageLoadingException("\"Order\" attribute must be different");

        try
        {
            foreach (var item in package.Items)
            {
                ValidateWorker(item.Worker);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }


    }
    private void ValidateWorker(Worker worker)
    {
        var plugin = TestPackage.Modules.FirstOrDefault(x => x.Name == worker.Alias);

        if (plugin == null) throw new PackageLoadingException("Invalid worker alias!");

        var typeName = plugin.QualifiedName.Split(',').First();
        var t = TypeParser.SearchType(typeName, plugin.Path);
        var m = t.GetMethod(worker.MethodName);
        if (m == null) throw new PackageLoadingException("Invalid worker method!");
    }
    private void ConfigureServices(TestPackage package)
    {
        scopedServices?.Dispose();
        services.Clear();
        foreach (var plugin in package.Modules)
        {
            var typeName = plugin.QualifiedName.Split(',').First();
            var t = TypeParser.SearchType(typeName, plugin.Path);
            services.AddKeyedScoped(plugin.Name, (s, k) =>
            {
                var obj = (IModule)Activator.CreateInstance(t);
                obj.Alias = k.ToString();
                return obj;
            });
        }

        scopedServices = services.BuildServiceProvider().CreateScope();


    }
    ~TestRunner()
    {
        services.Clear();
        scopedServices.Dispose();
    }
}

