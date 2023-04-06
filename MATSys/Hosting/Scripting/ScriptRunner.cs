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
        public object Configuration { get; set; }
        public ITestPackage TestPackage { get; }
        public JsonNode TestItems { get; set; }
        public ScriptRunner(IServiceProvider provider)
        {
            _appsettings = provider.GetRequiredService<IConfiguration>();
            var runnerSection = _appsettings.GetSection("MATSys:Runner");
            _config = runnerSection.Get<ScriptConfiguration>();
            Configuration = _config;
            TestItems = _config.TestItems;
            var testPackageFactory = provider.GetRequiredService<TestPackageFactory>();
            var tp = testPackageFactory.CreateTestPackage(runnerSection.GetSection("TestPackage"));
            TestPackage = tp;
        }
        public JsonArray RunTest(CancellationToken token = default)
        {
            _localCts = new CancellationTokenSource();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
            BeforeScriptStarts?.Invoke(TestItems);
            var arr = InternalItemExecution(TestItems.AsArray(), linkedTokenSource.Token);
            AfterScriptStops?.Invoke(arr);
            return arr;
        }
        public async Task<JsonArray> RunTestAsync(CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                return RunTest(token);
            });
        }

        public void StopTest()
        {
            _localCts.Cancel();
        }

        public void Export(string scriptPath)
        {
            var runnerSection = _appsettings.GetSection("MATSys:Runner");
            runnerSection["ScriptPath"] = scriptPath;
            File.WriteAllText(Path.GetFullPath(scriptPath), JsonSerializer.Serialize(TestItems));
        }

        public void Dispose()
        {
            TestPackage.Dispose();
        }

        public JsonArray RunTest(JsonNode testItems, CancellationToken token = default)
        {
            _localCts = new CancellationTokenSource();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
            BeforeScriptStarts?.Invoke(testItems);
            var arr = InternalItemExecution(testItems, linkedTokenSource.Token);
            AfterScriptStops?.Invoke(arr);
            return arr;
        }

        public JsonArray RunTest(string scriptFilePath, CancellationToken token = default)
        {
            _localCts = new CancellationTokenSource();
            JsonNode testItems = JsonArray.Parse(File.ReadAllText(scriptFilePath));

            _localCts = new CancellationTokenSource();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
            BeforeScriptStarts?.Invoke(testItems);
            var arr = InternalItemExecution(testItems, linkedTokenSource.Token);
            AfterScriptStops?.Invoke(arr);
            return arr;
        }
        public async Task<JsonArray> RunTestAsync(JsonNode testItems, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                return RunTest(testItems, token);
            });
        }
        public async Task<JsonArray> RunTestAsync(string scriptFilePath, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                return RunTest(scriptFilePath, token);
            });
        }

        private JsonArray InternalItemExecution(JsonNode items, CancellationToken token)
        {
            JsonArray arr = new JsonArray();
            foreach (var item in TestItems.AsArray())
            {
                if (token.IsCancellationRequested)
                {
                    return arr;
                }
                else
                {
                    var method = item["Method"].GetValue<string>();
                    var param = item["Param"];
                    BeforeTestItemStarts?.Invoke(item);
                    var result = TestPackage.Execute(method, param);
                    AfterTestItemStops?.Invoke(item, result);
                    arr.Add(result);
                }
            }
            return arr;

        }
    }


    internal class ScriptConfiguration
    {
        public string ScriptPath { get; set; }

        public JsonNode TestItems => JsonArray.Parse(File.ReadAllText(ScriptPath));
    }

}
