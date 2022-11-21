using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MATSys.Hosting.Scripting
{
    public class AutomationTestScriptContext
    {
        public static IEnumerable<MethodInfo> AnalyzerExtMethods { get; set; }
        public string RootDirectory { get; private set; } = @".\scripts";
        public List<TestItem> Setup { get; private set; } = new List<TestItem>();
        public List<TestItem> Test { get; private set; } = new List<TestItem>();
        public List<TestItem> Teardown { get; private set; } = new List<TestItem>();
        public AutomationTestScriptContext()
        {
            AnalyzerExtMethods = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => GetExtensionMethods<AnalyzingData>(x));
            
            try
            {
                var content = JsonNode.Parse(File.ReadAllText("appsettings.json"));
                var scriptSection = content["MATSys"]["Scripts"];
                if (scriptSection != null)
                {
                    if (scriptSection.AsObject().ContainsKey("RootDirectory"))
                    {
                        RootDirectory = scriptSection["RootDirectory"].GetValue<string>();
                    }
                    if (scriptSection.AsObject().ContainsKey("Setup"))
                    {
                        //load setup items
                        var setups = scriptSection["Setup"].AsArray();
                        Setup.AddRange(ParseItem(setups));
                    }
                    if (scriptSection.AsObject().ContainsKey("Test"))
                    {
                        //load Test items
                        var test = scriptSection["Test"].AsArray();
                        Test.AddRange(ParseItem(test));
                    }
                    if (scriptSection.AsObject().ContainsKey("Teardown"))
                    {
                        //load Teardown items
                        var teardown = scriptSection["Teardown"].AsArray();
                        Teardown.AddRange(ParseItem(teardown));
                    }

                }

            }
            catch (NullReferenceException ex)
            {
                throw ex;
            }
        }
        private List<TestItem> ParseItem(JsonArray array)
        {
            List<TestItem> list = new List<TestItem>();
            foreach (var item in array)
            {
                if (item.AsObject().ContainsKey("Executer"))
                {
                    list.Add(TestItem.Parse(item));
                }
                else if (item.AsObject().ContainsKey("Script"))
                {
                    var p = item["Script"].GetValue<string>();
                    list.AddRange(ReadFromScriptFile(p));
                }
            }
            return list;
        }
        public IEnumerable<TestItem> ReadFromScriptFile(string path)
        {
            var p = Path.Combine(RootDirectory, path);
            var content = JsonNode.Parse(File.ReadAllText(p));

            foreach (var item in content.AsArray())
            {
                if (item.AsObject().ContainsKey("Executer"))
                {
                    yield return TestItem.Parse(item);
                }
                else if (item.AsObject().ContainsKey("Script"))
                {
                    var subPath = item["Script"].GetValue<string>();
                    foreach (var subItem in ReadFromScriptFile(subPath))
                    {
                        yield return subItem;
                    }
                }
            }

        }
        private static IEnumerable<MethodInfo> GetExtensionMethods<T>(Assembly assembly)
        {
            var query = from type in assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == typeof(T)
                        select method;
            return query;
        }
    }
}
