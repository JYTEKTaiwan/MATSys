using System.Text.Json.Nodes;

namespace MATSys.Hosting
{
    /// <summary>
    /// ITestPackge
    /// </summary>
    public interface ITestPackage : IDisposable
    {

        CancellationToken ExecutionToken { get; set; }

        /// <summary>
        /// Alias of the TeatPacakge
        /// </summary>
        string? Alias { get; set; }
        /// <summary>
        /// ServiceProvider from the Host
        /// </summary>
        IServiceProvider? Provider { get; }
        /// <summary>
        /// INotifier instance, used to publish/subscribe message
        /// </summary>
        INotifier Notifier { get; }
        /// <summary>
        /// IRecorder instance, used to record the data locally
        /// </summary>
        IRecorder Recorder { get; }
        /// <summary>
        /// Type of the TeatPacakge
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// Execute the method that marked with <see cref="TestItemParameterAttribute"/>
        /// </summary>
        /// <param name="testItemName">method name</param>
        /// <param name="parameter">parameter in jsonnode format</param>
        /// <returns></returns>
        IResult Execute(string testItemName, JsonNode parameter);

        /// <summary>
        /// Inject the service provider of Host into the TestPackage
        /// </summary>
        /// <param name="serviceProvider">service provider fronm Host</param>
        void InjectServiceProvider(IServiceProvider serviceProvider);
        /// <summary>
        /// Inject the INotifier instance
        /// </summary>
        /// <param name="notifier">INotifier instance</param>
        void InjectNotifier(INotifier notifier);

        /// <summary>
        /// Inject the IRecorder instance
        /// </summary>
        /// <param name="recorder">IRecorder instance</param>
        void InjectRecorder(IRecorder recorder);

        void Setup();
        void Teardown();
    }
}
