using MATSys.Commands;

namespace MATSys.Modules
{
    public interface ITransceiver : IPlugin
    {
        delegate string CommandReadyEvent(object sender, string commandObjectInJson);
        event CommandReadyEvent? OnCommandReady;

    }
}
