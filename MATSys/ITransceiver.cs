namespace MATSys
{
    public interface ITransceiver : IPlugin
    {
        delegate string CommandReadyEvent(object sender, string commandObjectInJson);
        event CommandReadyEvent? OnCommandReady;

    }
}
