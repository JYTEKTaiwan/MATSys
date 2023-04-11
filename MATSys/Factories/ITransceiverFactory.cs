using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    /// <summary>
    /// Interface for TransceiverFactory
    /// </summary>
    public interface ITransceiverFactory
    {
        /// <summary>
        /// Create Transceiver from configuration file
        /// </summary>
        /// <param name="section">Section of the configuration file</param>
        /// <returns>ITransceiver instance</returns>
        ITransceiver CreateTransceiver(IConfigurationSection section);
    }
}