
namespace MATSys
{
    /// <summary>
    /// Interface of Recorder
    /// </summary>
    public interface IRecorder : IPlugin
    {
        /// <summary>
        /// Write data to the instance
        /// </summary>
        /// <param name="data">Data to be written</param>
        void Write(object data);

        /// <summary>
        /// Write data to the instance asynchronuously
        /// </summary>
        /// <param name="data">Data to be written</param>
        Task WriteAsync(object data);
    }
}