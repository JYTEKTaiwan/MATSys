namespace MATSys
{
    /// <summary>
    /// Interface for Module
    /// </summary>
    public interface IModule : IService
    {
        /// <summary>
        /// Indicate new data is generated from module
        /// </summary>
        /// <param name="jsonString"></param>
        delegate void NewDataReady(string jsonString);

        /// <summary>
        /// Event when object is disposed
        /// </summary>
        event EventHandler? IsDisposed;

        /// <summary>
        /// Notifier instance
        /// </summary>
        INotifier Notifier { get; }

        /// <summary>
        /// Logger instance
        /// </summary>
        NLog.ILogger? Logger { get; }
        /// <summary>
        /// Recorder instance
        /// </summary>
        IRecorder Recorder { get; }
        /// <summary>
        /// Transceiver instance
        /// </summary>
        ITransceiver Transceiver { get; }

        /// <summary>
        /// Indicate whether instance is running
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// Self instance
        /// </summary>
        IModule Base { get; }

        IServiceProvider Provider { get; }
        /// <summary>
        /// Execute the assigned command
        /// </summary>
        /// <param name="cmd">ICommand instance</param>
        /// <returns>Serialized response</returns>
        string Execute(Commands.ICommand cmd);


        string ExecuteCommandString(string cmdInJson);

#if NET6_0_OR_GREATER||NETSTANDARD2_0
        /// <summary>
        /// Asynchronously execute the assigned command
        /// </summary>
        /// <param name="cmd">ICommand instance</param>
        /// <returns>Serialized response</returns>
        Task<string> ExecuteAsync(Commands.ICommand cmd);

        Task<string> ExecuteAsync(string methodName, params object[] parameters);
#endif


        string Execute(string methodName, params object[] parameters);

        /// <summary>
        /// Configurae instance with object 
        /// </summary>
        /// <param name="config">configuration object</param>
        void Configure(object? config);

        /// <summary>
        /// Inject the IRecorder instance into IModule instance
        /// </summary>
        /// <param name="recorder">IRecorder instance</param>
        void InjectPlugin(IRecorder recorder);
        /// <summary>
        /// Inject the ITransceiver instance into IModule instance
        /// </summary>
        /// <param name="transceiver">ITransceiver instance</param>
        void InjectPlugin(ITransceiver transceiver);
        /// <summary>
        /// Inject the INotifier instance into IModule instance
        /// </summary>
        /// <param name="notifier">INotifier instance </param>
        void InjectPlugin(INotifier notifier);

        void SetProvider(IServiceProvider provider);

        /// <summary>
        /// Print all commands that is marked with MATSysCommandAttribute
        /// </summary>
        /// <returns>Collection of string</returns>
        IEnumerable<string> PrintCommands();
    }


}