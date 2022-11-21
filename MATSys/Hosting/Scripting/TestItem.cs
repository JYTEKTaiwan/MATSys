using MATSys.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MATSys.Hosting.Scripting
{
    /// <summary>
    /// Class represents the single test item 
    /// </summary>
    public class TestItem
    {

        private const string ExecuterSection = "Executer";
        private const string AnalyzerSection = "Analyzer";
        private const string LoopProperty = "Loop";
        private const string RetryProperty = "Retry";
        private JsonNode _ctxt;

        private JsonSerializerOptions opt = new JsonSerializerOptions()
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>
        /// The Unique ID for the test item
        /// </summary>
        public int UID { get; set; }
        /// <summary>
        /// The instance that store the command 
        /// </summary>
        public HubCommand? Executer { get; set; }
        /// <summary>
        /// The invoker that store the post-process information
        /// </summary>
        public MethodInvoker? Analyzer { get; set; }

        /// <summary>
        /// Parameters that is used for Analyzer property
        /// </summary>
        public object[]? AnalyzerParameter { get; set; }
        /// <summary>
        /// Loop setting(prior to Retry)
        /// </summary>
        public int Loop { get; set; } = -1;
        /// <summary>
        /// Retry setting
        /// </summary>
        public int Retry { get; set; } = -1;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="node">Information from JsonNode</param>
        public TestItem(JsonNode node)
        {
            UID = this.GetHashCode();
            _ctxt = node;
            if (node.AsObject().ContainsKey(LoopProperty))
            {
                //Loop property
                Loop = node[LoopProperty].Deserialize<int>();
            }
            if (node.AsObject().ContainsKey(RetryProperty))
            {
                //Retry property
                Retry = node[RetryProperty].Deserialize<int>();
            }
            if (node.AsObject().ContainsKey(ExecuterSection))
            {
                //Executer property
                Executer = new HubCommand()
                {
                    ModuleName = node.AsObject()[ExecuterSection]!.AsObject().First().Key,
                    CommandString = node.AsObject()[ExecuterSection]!.AsObject().First().Value!
                };
            }
            if (node.AsObject().ContainsKey(AnalyzerSection))
            {
                //Analyzer property
                var name = node.AsObject()[AnalyzerSection]!.AsObject().First().Key;
                var param = node.AsObject()[AnalyzerSection]!.AsObject().First().Value!.AsArray();
                var mi = AutomationTestScriptContext.AnalyzerExtMethods.FirstOrDefault(x => x.Name == name);
                Analyzer = MethodInvoker.Create(mi);
                var types = mi.GetParameters().Select(x => x.ParameterType).ToArray();
                AnalyzerParameter = new object[param.Count + 1];
                if (param.Count!=0)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        AnalyzerParameter[i + 1] = param[i].Deserialize(types[i + 1])!;
                    }
                }
                
            }


        }
        /// <summary>
        /// Parse from JsonNode
        /// </summary>
        /// <param name="node">JsonNode</param>
        /// <returns>TestItem instance</returns>
        public static TestItem Parse(JsonNode node)
        {
            if (node!=null)
            {
                return new TestItem(node);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Get the Executer Jsonobject
        /// </summary>
        /// <returns>JsonObject instance</returns>
        public JsonObject GetExecuter()
        {
            return _ctxt.AsObject()[ExecuterSection]!.AsObject();
        }
        /// <summary>
        /// Get the Analyzer Jsonobject
        /// </summary>
        /// <returns>JsonObject instance</returns>
        public JsonObject GetAnalyzer()
        {
            return _ctxt.AsObject()[AnalyzerSection]!.AsObject();
        }
    }
}
