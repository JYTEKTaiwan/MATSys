using MATSys.Commands;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MATSys.Hosting.Scripting
{
    public class ScriptRunner : IRunner
    {
        private delegate JsonNode TestItemRunner(TestItem item);

        private CancellationTokenSource _localCts = new CancellationTokenSource();
        protected static JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        private TestItemRunner _runner;
        public event IRunner.ReadyToExecuteScriptEvent? BeforeScriptStarts;
        public event IRunner.ReadyToExecuteTestItemEvent? BeforeTestItemStarts;
        public event IRunner.ExecuteTestItemCompleteEvent? AfterTestItemStops;
        public event IRunner.ExecuteScriptCompleteEvent? AfterScriptStops;
        public event IRunner.ExecuteSubTestItemCompleteEvent? AfterSubTestItemComplete;

        private Dictionary<string, IModule> _modulesInHub;
        public TestScriptContext TestScript { get; set; }
        public string Execute(string modName, string cmdInJson)
        {
            throw new NotImplementedException("This runner only supports scripting, use RunTest instead.");

        }

        public string Execute(string modName, ICommand cmd)
        {
            throw new NotImplementedException("This runner only supports scripting, use RunTest instead.");
        }

        public JsonArray RunTest(int iteration = 1)
        {
            _localCts = new CancellationTokenSource();
            JsonArray response = new JsonArray();
            BeforeScriptStarts?.Invoke(TestScript);
            foreach (var item in TestScript.Setup)
            {
                var res = ExecuteTestItem(item, _localCts.Token);
                if (res != null)
                {
                    response.Add(res);
                }
            }
            for (int i = 0; i < iteration; i++)
            {
                if (!_localCts.IsCancellationRequested)
                {
                    foreach (var item in TestScript.Test)
                    {
                        var res = ExecuteTestItem(item, _localCts.Token);
                        if (res != null)
                        {
                            response.Add(res);
                        }
                    }
                }
                else
                {
                    break;
                }
                SpinWait.SpinUntil(() => false, 0);
            }
            foreach (var item in TestScript.Teardown)
            {
                var res = ExecuteTestItem(item, _localCts.Token);
                if (res != null)
                {
                    response.Add(res);
                }

            }
            AfterScriptStops?.Invoke(response);
            return response;
        }
        public async Task<JsonArray> RunTestAsync(int iteration = 1)
        {
            return await Task.Run(() =>
            {
                return RunTest(iteration);
            });
        }

        private JsonNode ExecuteTestItem(TestItem item, CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                BeforeTestItemStarts?.Invoke(item);
                _runner = ChooseTestItemRunner(item);
                var result = _runner.Invoke(item);
                AfterTestItemStops?.Invoke(item, result);
                return result;
            }
            else
            {
                return null;
            }

        }
        private TestItemRunner ChooseTestItemRunner(TestItem item)
        {
            //if Loop and Retry both exists, ignore retry
            if (item.Loop > 0)
            {
                //case that loop count larger than 0 (retry will be ignored)
                return LoopTest;
            }
            else if (item.Retry > 0)
            {
                //case that retry count is larger than 0
                return RetryTest;
            }
            else
            {
                return SingleTest;
            }
        }
        public void StopTest()
        {
            _localCts.Cancel();
        }

        private JsonNode LoopTest(TestItem item)
        {
            (bool isPassed, JsonNode result) node = ValueTuple.Create(false, new JsonObject() as JsonNode);
            for (int i = 0; i < item.Loop; i++)
            {
                var ans = Execute(item);
                node = Analyze(item, ans, $"Loop: {i + 1}/{item.Loop}");
                AfterSubTestItemComplete?.Invoke(item, node.result);
                if (!node.isPassed)
                {
                    break;
                }
                if (_localCts.IsCancellationRequested)
                {
                    break;
                }
                SpinWait.SpinUntil(() => false, 0);
            }
            return node.result;
        }
        private JsonNode RetryTest(TestItem item)
        {
            (bool isPassed, JsonNode result) node = ValueTuple.Create(false, new JsonObject() as JsonNode);
            for (int i = 0; i < item.Retry; i++)
            {
                var ans = Execute(item);
                node = Analyze(item, ans, $"Retry: {i + 1}/{item.Retry}");
                AfterSubTestItemComplete?.Invoke(item, node.result);
                if (node.isPassed)
                {
                    break;
                }
                if (_localCts.IsCancellationRequested)
                {
                    break;
                }
                SpinWait.SpinUntil(() => false, 0);
            }
            return node.result;
        }
        private JsonNode SingleTest(TestItem item)
        {
            if (!_localCts.IsCancellationRequested)
            {
                var ans = Execute(item);
                var node = Analyze(item, ans, null);
                AfterSubTestItemComplete?.Invoke(item, node.result);
                return node.result;
            }
            else
            {
                return new JsonObject();
            }
        }
        private string Execute(TestItem item)
        {
            var cmd = item.Executer.Value;
            var res = _modulesInHub[cmd.ModuleName].Execute(cmd.CommandString.ToJsonString());
            return res;
        }
        private (bool isPassed, JsonNode result) Analyze(TestItem item, string execValue, object attributesToWrite)
        {
            var skipAnalyzer = item.Analyzer == null;
            TestItemResult result;
            bool valid = false;
            if (!skipAnalyzer)
            {
                item.AnalyzerParameter[0] = AnalyzingData.Create(execValue);
                valid = (bool)item.Analyzer.Invoke(item.AnalyzerParameter);
                var testResult = valid ? TestResultType.Pass : TestResultType.Fail;
                result = TestItemResult.Create(item,testResult, execValue, attributesToWrite);
            }
            else
            {
                result = TestItemResult.Create(item,TestResultType.Skip, execValue, attributesToWrite);
            }
            var element = JsonSerializer.SerializeToNode(result, typeof(TestItemResult), options);
            return (valid, element);
        }

        public void Load(JsonNode section)
        {

            TestScript = new TestScriptContext(section);
        }

        public void InjectModules(Dictionary<string, IModule> mods)
        {
            _modulesInHub = mods;
        }
    }
}
