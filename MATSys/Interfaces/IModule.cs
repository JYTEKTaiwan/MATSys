namespace MATSys
{
    /// <summary>
    /// Interface for Module
    /// </summary>
    public interface IModule : IService
    {
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
        object Execute(Commands.ICommand cmd);
        object Execute(string methodName, params object[] parameters);
        void Execute(string cmdInJson, out object response);


#if NET6_0_OR_GREATER||NETSTANDARD2_0
        /// <summary>
        /// Asynchronously execute the assigned command
        /// </summary>
        /// <param name="cmd">ICommand instance</param>
        /// <returns>Serialized response</returns>
        Task<object> ExecuteAsync(Commands.ICommand cmd);

        Task<object> ExecuteAsync(string methodName, params object[] parameters);

#endif




        /// <summary>
        /// Configurae instance with object 
        /// </summary>
        /// <param name="config">configuration object</param>
        void Configure();

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

    }


}