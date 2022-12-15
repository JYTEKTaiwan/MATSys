using System.Text.Json.Nodes;

namespace MATSys.Hosting.Scripting
{
    /// <summary>
    /// Struct that stores the Modules name and command
    /// </summary>
    public struct HubCommand
    {
        /// <summary>
        /// Name of the Module
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Command in Json format
        /// </summary>
        public JsonNode CommandString { get; set; }
    }
}
