﻿using MATSys.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MATSys.Hosting.Scripting
{
    public interface IRunner
    {
        AutomationTestScriptContext TestScript { get; internal set; }

        string Execute(string modName, string cmdInJson);
        string Execute(string modName, ICommand cmd);
        JsonArray RunTest(int iteration = 1);
        void StopTest();
        Task<JsonArray> RunTestAsync(int iteration = 1);
        void Load(IConfigurationSection section,AutomationTestScriptContext ts);        

        void InjectModules(Dictionary<string,IModule> mods);

        delegate void ReadyToExecuteScriptEvent(AutomationTestScriptContext script);

        event ReadyToExecuteScriptEvent? BeforeScriptStarts;

        delegate void ReadyToExecuteTestItemEvent(TestItem item);

        event ReadyToExecuteTestItemEvent? BeforeTestItemStarts;

        delegate void ExecuteTestItemCompleteEvent(TestItem item, JsonNode result);

        event ExecuteTestItemCompleteEvent? AfterTestItemStops;

        delegate void ExecuteScriptCompleteEvent(JsonArray item);

        event ExecuteScriptCompleteEvent? AfterScriptStops;
    }
}
