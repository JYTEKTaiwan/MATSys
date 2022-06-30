namespace MATSys
{    
        public interface IRecorder : IModule
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

    public interface IRecorder<in T> : IRecorder
    {
        void Write(T data);

        Task WriteAsync(T data);
    }
}