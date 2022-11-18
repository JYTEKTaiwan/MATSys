using MATSys.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MATSys.Hosting.Scripting
{
    public interface IRunner
    {

        string Execute(string modName, string cmdInJson);
        string Execute(string modName, ICommand cmd);
        void RunTest(int iteration = 1);
        void StopTest();
        Task RunTestAsync(int iteration = 1);

        delegate void ReadyToExecuteScriptEvent(AutomationTestScriptContext script);

        event ReadyToExecuteScriptEvent? BeforeScriptStarts;

        delegate void ReadyToExecuteTestItemEvent(TestItem item);

        event ReadyToExecuteTestItemEvent? BeforeTestItemStarts;

        delegate void ExecuteTestItemCompleteEvent(TestItem item, JsonNode result);

        event ExecuteTestItemCompleteEvent? AfterTestItemStops;

        delegate void ExecuteScriptCompleteEvent(IList<JsonNode> item);

        event ExecuteScriptCompleteEvent? AfterScriptStops;
    }
}
