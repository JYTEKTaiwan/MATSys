using MATSys.Commands;

namespace MATSys
{
    public interface ICommandServer:IModule
    {
        delegate string CommandReadyEvent(object sender, string commandObjectInJson);
        event CommandReadyEvent? OnCommandReady;
        
    }
}
