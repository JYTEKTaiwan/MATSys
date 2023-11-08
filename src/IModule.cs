using MATSys.Commands;
using Microsoft.Extensions.Configuration;


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
        /// Event the new data is generated
        /// </summary>
        event NewDataReady? OnDataReady;

        event EventHandler IsDisposed;

        IConfigurationSection Configuration{get;}

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

        /// <summary>
        /// Execute the assigned command
        /// </summary>
        /// <param name="cmd">ICommand instance</param>
        /// <returns>Response</returns>
        string Execute(Commands.ICommand cmd);

        Task<string> ExecuteAsync(Commands.ICommand cmd);
        /// <summary>
        /// Execute the assigned command
        /// </summary>
        /// <param name="cmdInJson">Command string</param>
        /// <returns>Response</returns>
        string Execute(string cmdInJson);

        /// <summary>
        /// Configurae IModule instance with object 
        /// </summary>
        /// <param name="config">configuration object</param>
        void Configure(object? config);
        /// <summary>
        /// Inject the IRecorder instance into IModule instance
        /// </summary>
        /// <param name="recorder">IRecorder instance</param>
        void InjectRecorder(IRecorder recorder);
        /// <summary>
        /// Inject the ITransceiver instance into IModule instance
        /// </summary>
        /// <param name="transceiver">ITransceiver instance</param>
        void InjectTransceiver(ITransceiver transceiver);
        /// <summary>
        /// Inject the INotifier instance into IModule instance
        /// </summary>
        /// <param name="notifier">INotifier instance </param>
        void InjectNotifier(INotifier notifier);
        /// <summary>
        /// Print all commands that is marked with MATSysCommandAttribute
        /// </summary>
        /// <returns>Collection of string</returns>
        IEnumerable<string> PrintCommands();
    }


}