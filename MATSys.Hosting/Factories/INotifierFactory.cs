using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    /// <summary>
    /// Interface for NotifierFactory
    /// </summary>
    public interface INotifierFactory
    {
        /// <summary>
        /// Create Notifier from configuration file
        /// </summary>
        /// <param name="section">Section of the configuration file</param>
        /// <returns>INotifier instance</returns>
        INotifier CreateNotifier(IConfigurationSection section);
    }
}