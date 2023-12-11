namespace MATSys
{
    /// <summary>
    /// Interface of Transceiver
    /// </summary>
    public interface ITransceiver : IPlugin
    {
        /// <summary>
        /// Indicate whether new request is happened
        /// </summary>
        /// <param name="sender">sender instance</param>
        /// <param name="commandObjectInJson">content of the request</param>
        /// <returns>Response for the request</returns>
        delegate string RequestFiredEvent(object sender, string commandObjectInJson);

        /// <summary>
        /// Event for the new coming request
        /// </summary>
        event RequestFiredEvent? OnNewRequest;

    }
}
