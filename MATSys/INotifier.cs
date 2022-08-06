namespace MATSys
{
    public interface INotifier : IPlugin
    {
        delegate void NotifyEvent(string dataInJson);
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