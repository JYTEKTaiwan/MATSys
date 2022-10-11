using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    /// <summary>
    /// Interface for RecorderFactory
    /// </summary>
    public interface IRecorderFactory
    {
        /// <summary>
        /// Create Recorder from configuration file
        /// </summary>
        /// <param name="info">Section of the configuration file</param>
        /// <returns>IRecorder instance</returns>
        IRecorder CreateRecorder(IConfigurationSection section);
    }
}