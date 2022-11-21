using MATSys.Commands;
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
    /// <summary>
    /// Interface of Runner instance
    /// </summary>
    public interface IRunner
    {

        /// <summary>
        /// Test Script context
        /// </summary>
        AutomationTestScriptContext TestScript { get; internal set; }
        /// <summary>
        /// (ONLY VALID IF ScriptMode is disabled) Execute command to specific module
        /// </summary>
        /// <param name="modName">Name of the Module</param>
        /// <param name="cmdInJson">Command in json string </param>
        /// <returns>Response from Module</returns>
        string Execute(string modName, string cmdInJson);
        /// <summary>
        /// (ONLY VALID IF ScriptMode is disabled) Execute command to specific module
        /// </summary>
        /// <param name="modName">Name of the Module</param>
        /// <param name="cmd">ICommand instance for command</param>
        /// <returns>Response from Module</returns>
        string Execute(string modName, ICommand cmd);

        /// <summary>
        /// (ONLY VALID IF ScriptMode is enabled) Run the script execution
        /// </summary>
        /// <param name="iteration">iteration of script</param>
        /// <returns>answer in JsonArray format</returns>
        JsonArray RunTest(int iteration = 1);
        /// <summary>
        /// Stop the script execution
        /// </summary>
        void StopTest();
        /// <summary>
        /// (ONLY VALID IF ScriptMode is enabled) Run the script execution asynchronously
        /// </summary>
        /// <param name="iteration">iteration of script</param>
        /// <returns>answer in JsonArray format</returns>        
        Task<JsonArray> RunTestAsync(int iteration = 1);
        
        /// <summary>
        /// Load configuration and test script context
        /// </summary>
        /// <param name="section"></param>
        /// <param name="ts"></param>
        void Load(IConfigurationSection section,AutomationTestScriptContext ts);        

        /// <summary>
        /// Inject the modules that created from Hub
        /// </summary>
        /// <param name="mods"></param>
        void InjectModules(Dictionary<string,IModule> mods);

        delegate void ReadyToExecuteScriptEvent(AutomationTestScriptContext script);
        delegate void ReadyToExecuteTestItemEvent(TestItem item);
        delegate void ExecuteTestItemCompleteEvent(TestItem item, JsonNode result);
        delegate void ExecuteScriptCompleteEvent(JsonArray item);

        /// <summary>
        /// Event before the script starts
        /// </summary>
        event ReadyToExecuteScriptEvent? BeforeScriptStarts;

        /// <summary>
        /// Event before the item starts
        /// </summary>
        event ReadyToExecuteTestItemEvent? BeforeTestItemStarts;

        /// <summary>
        /// Event after the item stops
        /// </summary>
        event ExecuteTestItemCompleteEvent? AfterTestItemStops;

        /// <summary>
        /// Event after the script stopts
        /// </summary>
        event ExecuteScriptCompleteEvent? AfterScriptStops;
    }
}
