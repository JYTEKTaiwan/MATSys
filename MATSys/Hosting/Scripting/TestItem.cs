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
        public int UID { get; set; }
        public HubCommand? Executer { get; set; }
        public MethodInvoker? Analyzer { get; set; }
        public object[] AnalyzerParameter { get; set; }
        public int Loop { get; set; } = -1;
        public int Retry { get; set; } = -1;
        public TestItem(JsonNode node)
        {
            UID = this.GetHashCode();
            _ctxt = node;
            if (node.AsObject().ContainsKey(LoopProperty))
            {
                Loop = node[LoopProperty].Deserialize<int>();
            }
            if (node.AsObject().ContainsKey(RetryProperty))
            {
                Retry = node[RetryProperty].Deserialize<int>();
            }
            if (node.AsObject().ContainsKey(ExecuterSection))
            {
                Executer = new HubCommand()
                {
                    ModuleName = node.AsObject()[ExecuterSection].AsObject().First().Key,
                    CommandString = node.AsObject()[ExecuterSection].AsObject().First().Value
                };
            }
            if (node.AsObject().ContainsKey(AnalyzerSection))
            {
                var name = node.AsObject()[AnalyzerSection].AsObject().First().Key;
                var param = node.AsObject()[AnalyzerSection].AsObject().First().Value.AsArray();
                var mi = AutomationTestScriptContext.AnalyzerExtMethods.FirstOrDefault(x => x.Name == name);
                Analyzer = MethodInvoker.Create(mi);
                var types = mi.GetParameters().Select(x => x.ParameterType).ToArray();
                AnalyzerParameter = new object[param.Count + 1];
                for (int i = 0; i < param.Count; i++)
                {
                    AnalyzerParameter[i+1] = param[i].Deserialize(types[i+1]);
                }
            }


        }
        public static TestItem Parse(JsonNode node)
        {
            return new TestItem(node);
        }
        public JsonObject GetExecuter()
        {
            return _ctxt.AsObject()[ExecuterSection].AsObject();
        }
        public JsonObject GetAnalyzer()
        {
            return _ctxt.AsObject()[AnalyzerSection].AsObject();
        }

        private static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly,
       Type extendedType)
        {
            var query = from type in assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == extendedType
                        select method;
            return query;
        }
    }
}
