using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using MATSys.Commands;
using static NetMQ.NetMQSelector;

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
        private AutomationTestScriptContext _testScript;
        public ScriptRunner(AutomationTestScriptContext testScript, Dictionary<string, IModule> handle)
        {
            _modulesInHub = handle;
            _testScript = testScript;
        }
        public string Execute(string modName, string cmdInJson)
        {
            return "[Warn] Service is in script mode";
        }

        public string Execute(string modName, ICommand cmd)
        {
            return "[Warn] Service is in script mode";
        }

        public void RunTest(int iteration = 1)
        {
            RunTestAsync(iteration).Wait();
        }
        public async Task RunTestAsync(int iteration = 1)
        {
            await Task.Run(() =>
            {
                _localCts = new CancellationTokenSource();
                IList<JsonNode> responses = new List<JsonNode>();
                BeforeScriptStarts?.Invoke(_testScript);
                foreach (var item in _testScript.Setup)
                {
                    _runner = ChooseRunner(item);
                    foreach (var ans in _runner.Invoke(item))
                    {
                        responses.Add(ans);
                    }
                    //Console.WriteLine(item.Executer.Value.CommandString.ToJsonString());
                }
                for (int i = 0; i < iteration; i++)
                {
                    foreach (var item in _testScript.Test)
                    {
                        _runner = ChooseRunner(item);
                        foreach (var ans in _runner.Invoke(item))
                        {
                            responses.Add(ans);
                        }                //Console.WriteLine(item.Executer.Value.CommandString.ToJsonString());
                    }
                    if (_localCts.IsCancellationRequested)
                    {
                        break;
                    }
                    SpinWait.SpinUntil(() => false, 0);
                }
                foreach (var item in _testScript.Teardown)
                {
                    _runner = ChooseRunner(item);
                    foreach (var ans in _runner.Invoke(item))
                    {
                        responses.Add(ans);
                    }
                    //Console.WriteLine(item.Executer.Value.CommandString.ToJsonString());
                }
                AfterScriptStops?.Invoke(responses);
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
                if (node.isPassed)
                {
                    response.Add(node.result);
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
        private (bool isPassed,JsonNode result) Analyze(TestItem item,string execResult,object attributesToWrite)
        {
            var skipAnalyzer = item.Analyzer == null;
            var len = skipAnalyzer ? 0 : item.AnalyzerParameter.Length;
            TestItemResult result;
            IList<JsonNode> response = new List<JsonNode>();
            bool valid = false;
            if (!skipAnalyzer)
            {
                item.AnalyzerParameter[0] = AnalyzeData.Create(execResult);
                valid = (bool)item.Analyzer.Invoke(item.AnalyzerParameter);
                var ans = valid ? TestResultType.Pass : TestResultType.Fail;
                result = TestItemResult.Create(ans, execResult, attributesToWrite);
            }
            else
            {
                result = TestItemResult.Create( TestResultType.Skip,execResult, attributesToWrite);
            }
            var node = JsonSerializer.SerializeToNode(result, result.GetType(), options);
            return (valid,node);
        }
    }
}
