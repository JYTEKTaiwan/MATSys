namespace MATSys
{
    public interface IDevice : IModule
    {
        delegate  void NewDataReady(string jsonString);
        event NewDataReady? OnDataReady;
        IDataBus DataBus {get;}
        NLog.ILogger Logger { get; }
         IDataRecorder DataRecorder { get; }
         ICommandServer Server { get; }
        string Execute(Commands.ICommand cmd);
        string Execute(string cmdInJson);

        IEnumerable<string> PrintCommands();
        IDevice Instance { get; }
    }
}