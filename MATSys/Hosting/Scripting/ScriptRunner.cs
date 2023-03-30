using MATSys.Commands;
using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MATSys.Hosting.Scripting
{
    //V2.0
    public class ScriptRunner : IRunner
    {
        private readonly IConfiguration _appsettings;
        private readonly ScriptConfiguration _config;
        private CancellationTokenSource _localCts = new CancellationTokenSource();
        protected static JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        public event IRunner.ReadyToExecuteScriptEvent? BeforeScriptStarts;
        public event IRunner.ReadyToExecuteTestItemEvent? BeforeTestItemStarts;
        public event IRunner.ExecuteTestItemCompleteEvent? AfterTestItemStops;
        public event IRunner.ExecuteScriptCompleteEvent? AfterScriptStops;
        public event IRunner.ExecuteSubTestItemCompleteEvent? AfterSubTestItemComplete;
        public object TestScript { get; set; }
        public Dictionary<string, ITestPackage> TestPackages { get;  set; } = new Dictionary<string, ITestPackage>();

        public ScriptRunner(IServiceProvider provider)
        {
            _appsettings = provider.GetRequiredService<IConfiguration>();
            var runnerSection = _appsettings.GetSection("MATSys:Runner");
            _config = runnerSection.Get<ScriptConfiguration>();
            TestScript = _config.TestItems;
            var testPackageFactory = provider.GetRequiredService<TestPackageFactory>();
            TestPackages = testPackageFactory.LoadTestPackagesFromSetting().ToDictionary(x=>x.Alias);
        }
        public JsonArray RunTest(int iteration = 1)
        {
            _localCts = new CancellationTokenSource();
            JsonArray arr = new JsonArray();
            foreach (var item in _config.TestItems.AsArray())
            {
                var target = item["Target"].GetValue<string>();
                var method = item["Method"].GetValue<string>();
                var param = item["Param"];
                arr.Add(TestPackages[target].Execute(method,param));

            }
            return arr;
        }
        public async Task<JsonArray> RunTestAsync(int iteration = 1)
        {
            return await Task.Run(() =>
            {
                return RunTest(iteration);
            });
        }

        public void StopTest()
        {
            _localCts.Cancel();
        }



    }


    internal class ScriptConfiguration
    {
        public string ScriptPath { get;  set; }

        public JsonNode TestItems => JsonArray.Parse(File.ReadAllText(ScriptPath));
    }

}
