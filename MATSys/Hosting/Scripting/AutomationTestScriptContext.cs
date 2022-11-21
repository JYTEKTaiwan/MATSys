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
    /// <summary>
    /// Context class for text loaded from json file
    /// </summary>
    public class AutomationTestScriptContext
    {
        /// <summary>
        /// Collection for extended methods of <see cref="AnalyzingData"/> 
        /// </summary>
        public static IEnumerable<MethodInfo> AnalyzerExtMethods { get; set; }

        /// <summary>
        /// Working directory for external script file
        /// </summary>
        public string RootDirectory { get; private set; } = @".\scripts";
        /// <summary>
        /// Setup items
        /// </summary>
        public List<TestItem> Setup { get; private set; } = new List<TestItem>();

        /// <summary>
        /// Test items
        /// </summary>
        public List<TestItem> Test { get; private set; } = new List<TestItem>();

       /// <summary>
       /// Teardown items
       /// </summary>
        public List<TestItem> Teardown { get; private set; } = new List<TestItem>();

        /// <summary>
        /// ctor
        /// </summary>
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

        /// <summary>
        /// Parse from JsonArray in json file
        /// </summary>
        /// <param name="array">Json Array</param>
        /// <returns>Collection of <see cref="TestItem"/></returns>
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
        /// <summary>
        /// Read the file and convert to TestItem colletion
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>Collection of <see cref="TestItem"/></returns>
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
            var types = assembly.GetTypes().Where(type=>type.IsSealed && !type.IsGenericType && !type.IsNested);
            foreach (var type in types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (method.IsDefined(typeof(ExtensionAttribute), false)&&
                        method.GetParameters()[0].ParameterType == typeof(T))
                    {
                        yield return method;
                    }
                }
            }            
        }
    }
}
