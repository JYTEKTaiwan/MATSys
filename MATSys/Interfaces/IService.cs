﻿namespace MATSys
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
        void Configure(object? config);


#if NET6_0_OR_GREATER||NETSTANDARD2_0
        /// <summary>
        /// Export the service insatnce into JObject format
        /// </summary>
        /// <returns>JObject instance</returns>
        System.Text.Json.Nodes.JsonObject Export();
#elif NET35
        /// <summary>
        /// Export the service insatnce into JObject format
        /// </summary>
        /// <returns>JObject instance</returns>
        Newtonsoft.Json.Linq.JObject Export();
#endif
        /// <summary>
        /// Export the service instance into json format
        /// </summary>
        /// <param name="indented">In indented format</param>
        /// <returns>string</returns>
        string Export(bool indented = true);

    }
}
