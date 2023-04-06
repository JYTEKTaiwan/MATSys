using System.Text.Json.Nodes;

namespace MATSys.Hosting.Scripting
{
    /// <summary>
    /// Interface of Runner instance
    /// </summary>
    public interface IRunner : IDisposable
    {
        ITestPackage TestPackage { get; }
        object Configuration { get; internal set; }
        JsonNode TestItems { get; set; }

        /// <summary>
        /// Run the script execution
        /// </summary>
        /// <param name="iteration">iteration of script</param>
        /// <returns>answer in JsonArray format</returns>
        JsonArray RunTest(CancellationToken token = default);

        JsonArray RunTest(JsonNode testItems, CancellationToken token = default);

        JsonArray RunTest(string scriptFilePath, CancellationToken token = default);

        /// <summary>
        /// Stop the script execution
        /// </summary>
        void StopTest();
        Task<JsonArray> RunTestAsync(CancellationToken token = default);
        Task<JsonArray> RunTestAsync(JsonNode testItems, CancellationToken token = default);
        Task<JsonArray> RunTestAsync(string scriptFilePath, CancellationToken token = default);




        delegate void ReadyToExecuteScriptEvent(JsonNode script);
        delegate void ReadyToExecuteTestItemEvent(JsonNode item);
        delegate void ExecuteTestItemCompleteEvent(JsonNode item, JsonNode result);
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
