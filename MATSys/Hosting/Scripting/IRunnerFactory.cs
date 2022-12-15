namespace MATSys.Hosting.Scripting
{
    /// <summary>
    /// Interface of factory that used to create runner
    /// </summary>
    internal interface IRunnerFactory
    {
        /// <summary>
        /// Create Runner instance
        /// </summary>
        /// <returns><see cref="IRunner"/> instance</returns>
        IRunner CreateRunner();

    }
}
