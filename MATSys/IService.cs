﻿using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;

namespace MATSys
{
    /// <summary>
    /// Interface of Service
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Name of the Service
        /// </summary>
        string Name { get; }
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
        /// Start the service
        /// </summary>
        /// <param name="token">Stop token</param>
        void StartService(CancellationToken token);

        /// <summary>
        /// Export the service insatnce into JObject format
        /// </summary>
        /// <returns>JObject instance</returns>
        JsonObject Export();

        /// <summary>
        /// Export the service instance into string format
        /// </summary>
        /// <param name="format">Format</param>
        /// <returns>string</returns>
        string Export(bool indented = true);
        /// <summary>
        /// Stop service
        /// </summary>
        void StopService();
    }
}
