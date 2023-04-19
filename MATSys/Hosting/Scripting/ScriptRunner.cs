using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace MATSys.Hosting.Scripting
{
    /// <summary>
    /// Script runner class is built-in class in the MATSys library. This is used to execute the testpackage items in sequence
    /// </summary>
    public class ScriptRunner : IRunner
    {

        private readonly IConfiguration _appsettings;
        private readonly ScriptConfiguration _config;
        private volatile bool isPaused = false;
        private volatile bool isRunning = false;
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
        /// Event when Pause is called during the execution
        /// </summary>
        public event EventHandler OnPause;

        /// <summary>
        /// Event when Resume is called during the execution
        /// </summary>
        public event EventHandler OnResume;

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
            if (runnerSection.Exists())
            {
                _config = runnerSection.Get<ScriptConfiguration>();
                Configuration = _config;
                TestItems = _config.LoadTestItems()!;
                var testpackageSection = runnerSection.GetSection("TestPackage");
                var testPackageFactory = provider.GetRequiredService<TestPackageFactory>();
                TestPackage = testpackageSection.Exists() ?
                    testPackageFactory.CreateTestPackage(testpackageSection)
                    : new EmptyTestPackage();
            }
            else
            {
                //create empty testpackage and configuration 
                Configuration = new ScriptConfiguration();
                TestPackage = new EmptyTestPackage();
            }

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
            return ExecutionLoop(TestItems, token);
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
            return ExecutionLoop(testItems, token);
        }
        /// <summary>
        /// Execute the test
        /// </summary>
        /// <param name="scriptFilePath">File of the test items located</param>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        public JsonArray RunTest(string scriptFilePath, CancellationToken token = default)
        {
            JsonNode testItems = JsonArray.Parse(File.ReadAllText(scriptFilePath))!;
            return ExecutionLoop(testItems, token);
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
        private JsonArray ExecutionLoop(JsonNode items, CancellationToken token)
        {
            JsonArray arr = new JsonArray();
            var itemEnumerator = items.AsArray().GetEnumerator();
            itemEnumerator.Reset();
            _localCts = new CancellationTokenSource();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
            isRunning = true;
            isPaused = false;
            BeforeScriptStarts?.Invoke(items);
            while (!linkedTokenSource.IsCancellationRequested)
            {
                if (isPaused)
                {
                    //paused
                }
                else if (itemEnumerator.MoveNext())
                {
                    var item = itemEnumerator.Current;
                    var method = item!["Method"]!.GetValue<string>();
                    var param = item["Param"]!;
                    BeforeTestItemStarts?.Invoke(item);
                    var result = TestPackage.Execute(method, param);
                    var node = JsonSerializer.SerializeToNode(result, result.GetType());
                    AfterTestItemStops?.Invoke(item, node);
                    arr.Add(node);
                }
                else
                {
                    //end of test items
                    linkedTokenSource.Cancel();
                }
                Thread.Sleep(1);
            }
            AfterScriptStops?.Invoke(arr);
            isRunning = false;
            return arr;
        }

        public void Pause()
        {
            if (isRunning)
            {
                isPaused = true;
                OnPause?.Invoke(this, null);
            }

        }

        public void Resume()
        {
            if (isRunning)
            {
                isPaused = false;
                OnResume?.Invoke(this, null);
            }

        }
    }


    internal class ScriptConfiguration
    {
        private JsonNode items;
        /// <summary>
        /// Path of the script
        /// </summary>
        public string ScriptPath { get; set; } = "";

        /// <summary>
        /// Get collection of the test items
        /// </summary>
        public JsonNode? LoadTestItems()
        {
            if (File.Exists(ScriptPath))
            {
                try
                {
                    return JsonArray.Parse(File.ReadAllText(ScriptPath));
                }
                catch (Exception)
                {
                    throw;
                }

            }
            else
            {
                return new JsonObject();
            }

        }


    }

}
