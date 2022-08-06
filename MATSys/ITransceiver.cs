namespace MATSys
{
    public interface ITransceiver : IPlugin
    {
        delegate string RequestFiredEvent(object sender, string commandObjectInJson);
        event RequestFiredEvent? OnNewRequest;

    }
}
