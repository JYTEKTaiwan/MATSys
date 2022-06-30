using MATSys.Commands;

namespace MATSys
{
    public interface ITransceiver:IModule
    {
        delegate string CommandReadyEvent(object sender, string commandObjectInJson);
        event CommandReadyEvent? OnCommandReady;
        
    }
}
