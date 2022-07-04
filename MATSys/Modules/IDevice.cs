namespace MATSys
{
    public interface IDevice : IModule
    {

        delegate void NewDataReady(string jsonString);
        event NewDataReady? OnDataReady;
        INotifier Notifier { get; }
        NLog.ILogger Logger { get; }
        IRecorder Recorder { get; }
        ITransceiver Transceiver { get; }

        bool IsRunning { get; }

        string Execute(Commands.ICommand cmd);
        string Execute(string cmdInJson);

        IEnumerable<string> PrintCommands();
        IDevice Instance { get; }
    }
}