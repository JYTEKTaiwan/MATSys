namespace MATSys
{
    public interface IDevice : IModule
    {

        delegate void NewDataReady(string jsonString);
        event NewDataReady? OnDataReady;
        INotifier DataBus { get; }
        NLog.ILogger Logger { get; }
        IRecorder DataRecorder { get; }
        ITransceiver Server { get; }

        bool IsRunning { get; }

        string Execute(Commands.ICommand cmd);
        string Execute(string cmdInJson);

        IEnumerable<string> PrintCommands();
        IDevice Instance { get; }
    }
}