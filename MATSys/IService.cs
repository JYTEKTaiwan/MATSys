namespace MATSys
{
    /// <summary>
    /// Interface of Service
    /// </summary>
    public interface IService : IDisposable
    {
        /// <summary>
        /// Name of the Service
        /// </summary>
        string Alias { get; set; }
        /// <summary>
        /// Load the configuration from file
        /// </summary>
        /// <param name="section">section of configuration filr</param>
        void Load(IConfigurationSection section);

        /// <summary>
        /// load the configuration from object
        /// </summary>
        /// <param name="configuration">configuration object</param>
        void Load(object configuration);


        /// <summary>
        /// Export the service insatnce into JObject format
        /// </summary>
        /// <returns>JObject instance</returns>
        JsonObject Export();

        /// <summary>
        /// Export the service instance into json format
        /// </summary>
        /// <param name="indented">In indented format</param>
        /// <returns>string</returns>
        string Export(bool indented = true);

    }
}
