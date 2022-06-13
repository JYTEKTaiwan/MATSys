namespace MATSys
{
    public interface IDataRecorder : IModule
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

    public interface IDataRecorder<in T> : IDataRecorder
    {
        void Write(T data);

        Task WriteAsync(T data);
    }
}