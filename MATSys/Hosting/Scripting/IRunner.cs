using System.Text.Json.Nodes;

namespace MATSys.Hosting.Scripting
{
    /// <summary>
    /// Interface of Runner instance
    /// </summary>
    public interface IRunner : IDisposable
    {
        /// <summary>
        /// TestPackage that embedded in this instance
        /// </summary>
        ITestPackage TestPackage { get; }
        /// <summary>
        /// Configuration instance loaded from settings
        /// </summary>
        object Configuration { get; set; }
        /// <summary>
        /// Test item collection
        /// </summary>
        JsonNode TestItems { get; set; }
        TaskStatus Status { get; }
        JsonNode CurrentItem { get; set; }

        JsonArray TestResults { get; set; }

        /// <summary>
        /// Stop the test
        /// </summary>
        void StopTest();

        /// <summary>
        /// Pause the execution
        /// </summary>
        void Pause();

        /// <summary>
        /// Resume the execution
        /// </summary>
        void Resume();

        void ResetStatus();
        
        /// <summary>
        /// Execute the test asynchronously
        /// </summary>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        Task<JsonArray> RunTestAsync(CancellationToken token = default);
        /// <summary>
        /// Execute the test asynchronously
        /// </summary>
        /// <param name="testItems">Collection of test items</param>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        Task<JsonArray> RunTestAsync(JsonNode testItems, CancellationToken token = default);
        /// <summary>
        /// Execute the test asynchronously
        /// </summary>
        /// <param name="scriptFilePath">File of the test items located</param>
        /// <param name="token">Cancelation token</param>
        /// <returns>The collection of result returned from each test item</returns>
        Task<JsonArray> RunTestAsync(string scriptFilePath, CancellationToken token = default);

        /// <summary>
        /// ReadyToExecuteScriptEvent
        /// </summary>
        /// <param name="script">script content</param>
        delegate void ReadyToExecuteScriptEvent(JsonNode? script);
        /// <summary>
        /// ReadyToExecuteTestItemEvent
        /// </summary>
        /// <param name="item">single test item</param>
        delegate void ReadyToExecuteTestItemEvent(JsonNode? item);
        /// <summary>
        /// ExecuteTestItemCompleteEvent
        /// </summary>
        /// <param name="item">Single test item</param>
        /// <param name="result">Result after execution</param>
        delegate void ExecuteTestItemCompleteEvent(JsonNode? item, JsonNode? result);
        /// <summary>
        /// ExecuteScriptCompleteEvent
        /// </summary>
        /// <param name="result">Result</param>
        delegate void ExecuteScriptCompleteEvent(JsonArray? result, TaskStatus status);

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

        /// <summary>
        /// Event when Pause is called during the execution
        /// </summary>
        event EventHandler OnPause;

        /// <summary>
        /// Event when Resume is called during the execution
        /// </summary>
        event EventHandler OnResume;
    }
}
