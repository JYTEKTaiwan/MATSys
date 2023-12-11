namespace MATSys
{
    /// <summary>
    /// Interface of Notifier
    /// </summary>
    public interface INotifier : IPlugin
    {
        /// <summary>
        /// Indicate that new data is ready to notify 
        /// </summary>
        /// <param name="dataInJson"></param>
        delegate void NotifyEvent(string dataInJson);

        /// <summary>
        /// Event when new data is ready to notify
        /// </summary>
        event NotifyEvent? OnNotify;
        /// <summary>
        /// Publish the data
        /// </summary>
        /// <param name="data">Value of the data</param>
        void Publish(object data);
        /// <summary>
        /// Get the latest data stored in the buffer. Return null if timeout
        /// </summary>
        /// <param name="timeoutInMilliseconds">Timeout value in milliseconds</param>
        object? GetData(int timeoutInMilliseconds = 1000);

    }


}