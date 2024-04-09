namespace MATSys
{
    /// <summary>
    /// Indicate whether new request is happened
    /// </summary>
    /// <param name="sender">sender instance</param>
    /// <param name="commandObjectInJson">content of the request</param>
    /// <returns>Response for the request</returns>
    public delegate string RequestFiredEvent(object sender, string commandObjectInJson);

    /// <summary>
    /// Interface of Transceiver
    /// </summary>
    public interface ITransceiver : IPlugin
    {
        /// <summary>
        /// Event for the new coming request
        /// </summary>
        event RequestFiredEvent? RequestReceived;

    }
}
