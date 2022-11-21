using MATSys.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
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
        public AutomationTestScriptContext TestScript { get; set; }


        public string Execute(string modName, string cmdInJson)
        {
            return _modulesInHub[modName].Execute(cmdInJson);
        }

        public string Execute(string modName, ICommand cmd)
        {
            return _modulesInHub[modName].Execute(cmd);
        }

        public JsonArray RunTest(int iteration = 1)
        {
            return null;
        }

        public void StopTest()
        {

        }

        public Task<JsonArray> RunTestAsync(int iteration = 1)
        {
            return Task<JsonArray>.Run(() => new JsonArray());
        }

        public void Load(IConfigurationSection section, AutomationTestScriptContext ts)
        {
        }

        public void InjectModules(Dictionary<string, IModule> mods)
        {
            _modulesInHub = mods;
        }
    }
}
