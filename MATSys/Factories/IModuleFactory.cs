using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    /// <summary>
    /// Interface for ModuleFactory
    /// </summary>
    public interface IModuleFactory
    {
        /// <summary>
        /// Create Module from configuration file
        /// </summary>
        /// <param name="info">Section of the configuration file</param>
        /// <returns>IModule instance</returns>
        IModule CreateModule(IConfigurationSection info);

    }
}