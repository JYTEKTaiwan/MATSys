
/* Unmerged change from project 'MATSys (netstandard2.0)'
Before:
using Microsoft.Extensions.Configuration;
After:
using MATSys;
using MATSys;
using MATSys.Modules;
using Microsoft.Extensions.Configuration;
*/
using Microsoft.Extensions.Configuration;

namespace MATSys
{
    public interface IModule : IService
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
        IModule Base { get; }

    }
}