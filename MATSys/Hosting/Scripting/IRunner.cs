using MATSys.Commands;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;

namespace MATSys.Hosting.Scripting
{
    /// <summary>
    /// Interface of Runner instance
    /// </summary>
    public interface IRunner
    {
        Dictionary<string, ITestPackage> TestPackages { get; set; }
        object TestScript { get; internal set; }

        /// <summary>
        /// Run the script execution
        /// </summary>
        /// <param name="iteration">iteration of script</param>
        /// <returns>answer in JsonArray format</returns>
        JsonArray RunTest(int iteration = 1);
        /// <summary>
        /// Stop the script execution
        /// </summary>
        void StopTest();
        /// <summary>
        /// Run the script execution asynchronously
        /// </summary>
        /// <param name="iteration">iteration of script</param>
        /// <returns>answer in JsonArray format</returns>        
        Task<JsonArray> RunTestAsync(int iteration = 1);




        delegate void ReadyToExecuteScriptEvent(TestScriptContext script);
        delegate void ReadyToExecuteTestItemEvent(TestItem item);
        delegate void ExecuteSubTestItemCompleteEvent(TestItem item, JsonNode result);
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
        /// Event when each sub test item completed
        /// </summary>
        event ExecuteSubTestItemCompleteEvent? AfterSubTestItemComplete;

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
