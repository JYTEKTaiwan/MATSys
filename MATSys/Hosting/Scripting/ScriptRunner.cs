using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;
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
        private Task<JsonArray> _task=Task.FromResult(new JsonArray());
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

        public TaskStatus Status => _task.Status;
        public JsonArray TestResults { get; set; }

        public JsonNode CurrentItem { get; set; }
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
        /// Execute the test asynchronously
        /// </summary>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        public virtual async Task<JsonArray> RunTestAsync(CancellationToken token = default)
        {
            return await ExecuteScriptAsync(token);
        }

        /// <summary>
        /// Execute the test asynchronously
        /// </summary>
        /// <param name="testItems">Collection of test items</param>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        public virtual async Task<JsonArray> RunTestAsync(JsonNode testItems, CancellationToken token = default)
        {
            TestItems = testItems;
            return await ExecuteScriptAsync(token);
        }
        /// <summary>
        /// Execute the test asynchronously
        /// </summary>
        /// <param name="scriptFilePath">File of the test items located</param>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        public virtual async Task<JsonArray> RunTestAsync(string scriptFilePath, CancellationToken token = default)
        {
            var TestItems = JsonArray.Parse(File.ReadAllText(scriptFilePath))!;
            return await ExecuteScriptAsync(token);
        }
        protected virtual async Task<JsonArray> ExecuteScriptAsync(CancellationToken token = default)
        {
            _localCts = new CancellationTokenSource();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
            var items = TestItems.AsArray();
            BeforeScriptStarts?.Invoke(items);
            _task = Task.Run(() =>
            {
                TestPackage.ExecutionToken = linkedTokenSource.Token;
                TestResults = new JsonArray();
                return TestResults;
            });
            foreach (var item in TestItems.AsArray())
            {
                var method = item!["Method"]!.GetValue<string>();
                var param = item["Param"]!;
                var node = new JsonObject() as JsonNode;
                _task = _task.ContinueWith(task =>
                {
                    CurrentItem = item;
                    BeforeTestItemStarts?.Invoke(item);
                    return TestResults;
                }, linkedTokenSource.Token);

                _task = _task.ContinueWith((task) =>
                {
                    var result = TestPackage.Execute(method, param);
                    node = JsonSerializer.SerializeToNode(result, result.GetType());
                    TestResults.Add(node);
                    return TestResults;
                }, linkedTokenSource.Token);

                _task = _task.ContinueWith(task =>
                {
                    AfterTestItemStops?.Invoke(item, node);
                    return TestResults;
                }, linkedTokenSource.Token);


            }
            /*
            for (int i = 0; i < items.Count; i++)
            {
                CurrentItem = items[i];
                var method = CurrentItem!["Method"]!.GetValue<string>();
                var param = CurrentItem["Param"]!;
                var node = new JsonObject() as JsonNode;
                _task = _task.ContinueWith(task =>
                {
                    BeforeTestItemStarts?.Invoke(i);
                    return TestResults;
                }, linkedTokenSource.Token);

                _task = _task.ContinueWith((task) =>
                {
                    var result =  TestPackage.Execute(method, param);
                    node = JsonSerializer.SerializeToNode(result, result.GetType());
                    TestResults.Add(node);
                    return TestResults;
                }, linkedTokenSource.Token);
                
                _task = _task.ContinueWith(task =>
                {
                    AfterTestItemStops?.Invoke(i, node);
                    return TestResults;
                }, linkedTokenSource.Token);

            }
            */
            _=_task.ContinueWith(task =>
            {
                AfterScriptStops?.Invoke(TestResults, task.Status);
                return TestResults;
            });
            return await _task;
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

        public void ResetStatus()
        {
            _task = Task.FromResult(new JsonArray());
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
