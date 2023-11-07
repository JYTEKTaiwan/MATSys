namespace MATSys
{
    /// <summary>
    /// Interface for MATSys configuration
    /// </summary>
    public interface IMATSysConfiguration
    {
        /// <summary>
        /// Type name of the instance
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Indicate whether logging feature is enabled
        /// </summary>
        bool EnableLogging { get; set; }

    }
}
