using MATSys.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Hosting.Scripting
{
    public class ManualRunner : IRunner
    {
        private Dictionary<string, IModule> _modulesInHub;

        public event IRunner.ReadyToExecuteScriptEvent? BeforeScriptStarts;
        public event IRunner.ReadyToExecuteTestItemEvent? BeforeTestItemStarts;
        public event IRunner.ExecuteTestItemCompleteEvent? AfterTestItemStops;
        public event IRunner.ExecuteScriptCompleteEvent? AfterScriptStops;

        public ManualRunner(Dictionary<string, IModule> modulesInHub)
        {
            _modulesInHub = modulesInHub;
        }

        public string Execute(string modName, string cmdInJson)
        {
            return _modulesInHub[modName].Execute(cmdInJson);
        }

        public string Execute(string modName, ICommand cmd)
        {
            return _modulesInHub[modName].Execute(cmd);
        }

        public void RunTest(int iteration = 1)
        {
        }

        public void StopTest()
        {

        }

        public Task RunTestAsync(int iteration = 1)
        {
            return Task.CompletedTask;
        }
    }
}
