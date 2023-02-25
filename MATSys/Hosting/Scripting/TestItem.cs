using MATSys.Commands;
using System.Text.Json;
using System.Text.Json.Nodes;

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


        public TestItem()
        {

        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="node">Information from JsonNode</param>
        public TestItem(JsonNode node)
        {
            UID = this.GetHashCode();
            _ctxt = node;

            //executer command must lies in 1st line
            //analyzer command must lies in 2nd line(optional)
            //retry/loop/repeat property is searchable (optional)
            //options property is searchale (optional)
            var enumerator = node.AsObject().AsEnumerable().GetEnumerator();
            var pair = enumerator.Current;

            //Executer section (check if jsonvalue type is jsonobject. if not, meaning executer section is not existed or in wrong format
            enumerator.MoveNext();
            pair = enumerator.Current;
            if (pair.Value.GetType() == typeof(JsonObject))
            {
                //executer seciton exists
                Executer = new HubCommand()
                {
                    ModuleName = pair.Key,
                    CommandString = pair.Value!
                };
            }
            else
            {
                //executer section is not exists
                throw new FormatException("Executer command does not exist or in wrong format");
            }

            //Analyzer section (check if jsonvalue type is jsonarray. if not, Set empty analyzer)
            enumerator.MoveNext();
            pair = enumerator.Current;
            if (pair.Value.GetType() == typeof(JsonArray))
            {
                //Analyzer property
                var name = node.AsObject()[AnalyzerSection]!.AsObject().First().Key;
                var param = node.AsObject()[AnalyzerSection]!.AsObject().First().Value!.AsArray();
                var mi = TestScriptContext.AnalyzerExtMethods.FirstOrDefault(x => x.Name == name);
                Analyzer = MethodInvoker.Create(mi);
                var types = mi.GetParameters().Select(x => x.ParameterType).ToArray();
                AnalyzerParameter = new object[param.Count + 1];
                if (param.Count != 0)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        AnalyzerParameter[i + 1] = param[i].Deserialize(types[i + 1])!;
                    }
                }

            }

            //Retry/Loop property configuration
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
        }
        /// <summary>
        /// Parse from JsonNode
        /// </summary>
        /// <param name="node">JsonNode</param>
        /// <returns>TestItem instance</returns>
        public static TestItem Parse(JsonNode node)
        {
            if (node != null)
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
            var obj = _ctxt.AsObject()[ExecuterSection];
            return obj == null ? new JsonObject() : obj.AsObject();
        }
        /// <summary>
        /// Get the Analyzer Jsonobject
        /// </summary>
        /// <returns>JsonObject instance</returns>
        public JsonObject GetAnalyzer()
        {
            var obj = _ctxt.AsObject()[AnalyzerSection];
            return obj == null ? new JsonObject() : obj.AsObject();
        }
    }
}
