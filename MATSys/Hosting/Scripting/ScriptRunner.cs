using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MATSys.Hosting.Scripting
{
    /// <summary>
    /// Script runner class is built-in class in the MATSys library. This is used to execute the testpackage items in sequence
    /// </summary>
    public class ScriptRunner : IRunner
    {

        private readonly IConfiguration _appsettings;
        private readonly ScriptConfiguration _config;
        private CancellationTokenSource _localCts = new CancellationTokenSource();

        /// <summary>
        /// json serialization options 
        /// </summary>
        protected static JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>
        /// Before the script starts Event
        /// </summary>
        public event IRunner.ReadyToExecuteScriptEvent? BeforeScriptStarts;
        /// <summary>
        /// Before the item start Event
        /// </summary>
        public event IRunner.ReadyToExecuteTestItemEvent? BeforeTestItemStarts;

        /// <summary>
        /// After the item completed Event
        /// </summary>
        public event IRunner.ExecuteTestItemCompleteEvent? AfterTestItemStops;
        /// <summary>
        /// After the script completed Event
        /// </summary>
        public event IRunner.ExecuteScriptCompleteEvent? AfterScriptStops;

        /// <summary>
        /// Configuration instance loaded from settings
        /// </summary>
        public object Configuration { get; set; }

        /// <summary>
        /// TestPackage that embedded in this instance
        /// </summary>
        public ITestPackage TestPackage { get; }
        /// <summary>
        /// Test item collection
        /// </summary>
        public JsonNode TestItems { get; set; } = new JsonObject();
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="provider">Service provider instance</param>
        public ScriptRunner(IServiceProvider provider)
        {
            _appsettings = provider.GetRequiredService<IConfiguration>();
            var runnerSection = _appsettings.GetSection("MATSys:Runner");
            _config = runnerSection.Get<ScriptConfiguration>();
            Configuration = _config;
            TestItems = _config.TestItems!;
            var testPackageFactory = provider.GetRequiredService<TestPackageFactory>();
            var tp = testPackageFactory.CreateTestPackage(runnerSection.GetSection("TestPackage"));
            TestPackage = tp;
        }
        /// <summary>
        /// Execute the test
        /// </summary>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        public JsonArray RunTest(CancellationToken token = default)
        {
            _localCts = new CancellationTokenSource();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
            BeforeScriptStarts?.Invoke(TestItems);
            var arr = InternalItemExecution(TestItems.AsArray(), linkedTokenSource.Token);
            AfterScriptStops?.Invoke(arr);
            return arr;
        }
        /// <summary>
        /// Execute the test asynchronously
        /// </summary>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        public async Task<JsonArray> RunTestAsync(CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                return RunTest(token);
            });
        }
        /// <summary>
        /// Stop the test
        /// </summary>
        public void StopTest()
        {
            _localCts.Cancel();
        }

        /// <summary>
        /// Export the test item into file
        /// </summary>
        /// <param name="scriptPath">export file path</param>
        public void Export(string scriptPath)
        {
            var runnerSection = _appsettings.GetSection("MATSys:Runner");
            runnerSection["ScriptPath"] = scriptPath;
            File.WriteAllText(Path.GetFullPath(scriptPath), JsonSerializer.Serialize(TestItems));
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            TestPackage.Dispose();
        }
        /// <summary>
        /// Execute the test
        /// </summary>
        /// <param name="testItems">Collection of test items</param>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        public JsonArray RunTest(JsonNode testItems, CancellationToken token = default)
        {
            _localCts = new CancellationTokenSource();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
            BeforeScriptStarts?.Invoke(testItems);
            var arr = InternalItemExecution(testItems, linkedTokenSource.Token);
            AfterScriptStops?.Invoke(arr);
            return arr;
        }
        /// <summary>
        /// Execute the test
        /// </summary>
        /// <param name="scriptFilePath">File of the test items located</param>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        public JsonArray RunTest(string scriptFilePath, CancellationToken token = default)
        {
            _localCts = new CancellationTokenSource();
            JsonNode testItems = JsonArray.Parse(File.ReadAllText(scriptFilePath))!;

            _localCts = new CancellationTokenSource();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
            BeforeScriptStarts?.Invoke(testItems);
            var arr = InternalItemExecution(testItems, linkedTokenSource.Token);
            AfterScriptStops?.Invoke(arr);
            return arr;
        }
        /// <summary>
        /// Execute the test asynchronously
        /// </summary>
        /// <param name="testItems">Collection of test items</param>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        public async Task<JsonArray> RunTestAsync(JsonNode testItems, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                return RunTest(testItems, token);
            });
        }
        /// <summary>
        /// Execute the test asynchronously
        /// </summary>
        /// <param name="scriptFilePath">File of the test items located</param>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
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
                    var method = item!["Method"]!.GetValue<string>();
                    var param = item["Param"]!;
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
        /// <summary>
        /// Path of the script
        /// </summary>
        public string ScriptPath { get; set; } = "";

        /// <summary>
        /// Collection of the test items
        /// </summary>
        public JsonNode? TestItems => JsonArray.Parse(File.ReadAllText(ScriptPath));
    }

}
