using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MATSys.Hosting.Scripting
{
    public struct HubCommand
    {
        public string ModuleName { get; set; }
        public JsonNode CommandString { get; set; }
    }
}
