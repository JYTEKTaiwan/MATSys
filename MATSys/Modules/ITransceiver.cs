using MATSys.Commands;

namespace MATSys
{
    public interface ITransceiver: IEmbededModule
    {
        delegate string CommandReadyEvent(object sender, string commandObjectInJson);
        event CommandReadyEvent? OnCommandReady;
        
    }
}
