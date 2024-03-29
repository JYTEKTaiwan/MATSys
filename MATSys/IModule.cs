﻿
namespace MATSys
{
    /// <summary>
    /// Interface for Module
    /// </summary>
    public interface IModule : IService
    {
        IServiceProvider Provider { get; set; }
        /// <summary>
        /// Indicate new data is generated from module
        /// </summary>
        /// <param name="jsonString"></param>
        delegate void NewDataReady(string jsonString);

        /// <summary>
        /// Event the new data is generated
        /// </summary>
        event NewDataReady? OnDataReady;

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
        /// Execute the assigned command
        /// </summary>
        /// <param name="cmd">ICommand instance</param>
        /// <returns>Response</returns>
        string Execute(Commands.ICommand cmd);
        /// <summary>
        /// Execute the assigned command
        /// </summary>
        /// <param name="cmd">Command string</param>
        /// <returns>Response</returns>
        string Execute(string cmdInJson);

        /// <summary>
        /// Print all commands that is marked with MATSysCommandAttribute
        /// </summary>
        /// <returns>Collection of string</returns>
        IEnumerable<string> PrintCommands();

        /// <summary>
        /// Self instance
        /// </summary>
        IModule Base { get; }

        /// <summary>
        /// Collection of IModule instances created from ModuleHubBackgroundService. Null if Module is manually created.
        /// </summary>
        Dictionary<string, IModule> LocalPeers { get; set; }

    }
}