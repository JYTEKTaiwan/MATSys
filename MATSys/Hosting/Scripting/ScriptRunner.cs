using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using MATSys.Commands;
using static NetMQ.NetMQSelector;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

namespace MATSys.Hosting.Scripting
{
    public class ScriptRunner : IRunner
    {
        private delegate IList<JsonNode> TestItemRunner(TestItem item);

        private CancellationTokenSource _localCts;
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
        private Dictionary<string,IModule> _modulesInHub;
        public AutomationTestScriptContext TestScript { get; set; }
        public string Execute(string modName, string cmdInJson)
        {
            throw new InvalidOperationException("Please DISABLE the ScripMode property in the configuration file");

        }

        public string Execute(string modName, ICommand cmd)
        {
            throw new InvalidOperationException("Please DISABLE the ScripMode property in the configuration file");
        }

        public JsonArray RunTest(int iteration = 1)
        {
            _localCts = new CancellationTokenSource();
            JsonArray response = new JsonArray();
            BeforeScriptStarts?.Invoke(TestScript);
            foreach (var item in TestScript.Setup)
            {
                _runner = ChooseRunner(item);
                foreach (var res in _runner.Invoke(item))
                {
                    response.Add(res);
                }
            }
            for (int i = 0; i < iteration; i++)
            {
                foreach (var item in TestScript.Test)
                {
                    _runner = ChooseRunner(item);
                    foreach (var res in _runner.Invoke(item))
                    {
                        response.Add(res);
                    }
                }
                if (_localCts.IsCancellationRequested)
                {
                    break;
                }
                SpinWait.SpinUntil(() => false, 0);
            }
            foreach (var item in TestScript.Teardown)
            {
                _runner = ChooseRunner(item);
                foreach (var res in _runner.Invoke(item))
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

        private TestItemRunner ChooseRunner(TestItem item)
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

        private IList<JsonNode> LoopTest(TestItem item)
        {
            IList<JsonNode> response = new List<JsonNode>();
            for (int i = 0; i < item.Loop; i++)
            {
                BeforeTestItemStarts?.Invoke(item);
                var ans = Execute(item);
                var node = Analyze(item, ans, $"Loop: {i+1}/{item.Loop}");
                AfterTestItemStops?.Invoke(item, node.result);
                response.Add(node.result);

                if (_localCts.IsCancellationRequested)
                {
                    break;
                }
                SpinWait.SpinUntil(() => false, 0);
            }
            return response;
        }
        private IList<JsonNode> RetryTest(TestItem item)
        {
            IList<JsonNode> response = new List<JsonNode>();
            for (int i = 0; i < item.Retry; i++)
            {
                BeforeTestItemStarts?.Invoke(item);
                var ans = Execute(item);
                var node = Analyze(item, ans, $"Retry: {i+1}/{item.Retry}");
                AfterTestItemStops?.Invoke(item, node.result);
                response.Add(node.result);
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
            return response;
        }
        private IList<JsonNode> SingleTest(TestItem item)
        {
            var response = new List<JsonNode>();
            BeforeTestItemStarts?.Invoke(item);
            var ans=Execute(item);
            var node = Analyze(item,ans,null);
            AfterTestItemStops?.Invoke(item, node.result);
            response.Add(node.result);
            return response;
        }
        private string Execute(TestItem item)
        {
            var cmd = item.Executer.Value;
            var res = _modulesInHub[cmd.ModuleName].Execute(cmd.CommandString.ToJsonString());
            return res;
        }
        private (bool isPassed,JsonNode result) Analyze(TestItem item,string execValue,object attributesToWrite)
        {
            var skipAnalyzer = item.Analyzer == null;
            var len = skipAnalyzer ? 0 : item.AnalyzerParameter.Length;
            TestItemResult result;
            bool valid = false;
            if (!skipAnalyzer)
            {
                item.AnalyzerParameter[0] = AnalyzingData.Create(execValue);
                valid = (bool)item.Analyzer.Invoke(item.AnalyzerParameter);
                var testResult = valid ? TestResultType.Pass : TestResultType.Fail;
                result = TestItemResult.Create(testResult, execValue, attributesToWrite);
            }
            else
            {
                result = TestItemResult.Create( TestResultType.Skip,execValue, attributesToWrite);
            }
            var element = JsonSerializer.SerializeToNode(result, typeof(TestItemResult),options);
            return (valid,element);
        }

        public void Load(IConfigurationSection section, AutomationTestScriptContext ts)
        {
            TestScript = ts;
        }

        public void InjectModules(Dictionary<string, IModule> mods)
        {
            _modulesInHub = mods;
        }
    }
}
