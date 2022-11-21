using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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
