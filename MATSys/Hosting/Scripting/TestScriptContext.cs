using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MATSys.Hosting.Scripting
{
    /// <summary>
    /// Context class for text loaded from json file
    /// </summary>
    public class TestScriptContext
    {
        /// <summary>
        /// Collection for extended methods of <see cref="AnalyzingData"/> 
        /// </summary>
        public static IEnumerable<MethodInfo> AnalyzerExtMethods { get; set; }

        private string _path = "";

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
        public TestScriptContext(JsonNode config)
        {
            AnalyzerExtMethods = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => GetExtensionMethods<AnalyzingData>(x));
            try
            {
                if (config != null)
                {
                    if (config["RootDirectory"] != null)
                    {
                        RootDirectory = config["RootDirectory"].GetValue<string>();
                    }
                    if (config["Setup"] != null)
                    {
                        //load setup items
                        var setups = config["Setup"].AsArray();
                        Setup.AddRange(ParseItems(setups));
                    }
                    if (config["Test"] != null)
                    {
                        //load Test items
                        var test = config["Test"].AsArray();
                        Test.AddRange(ParseItems(test));
                    }
                    if (config["Teardown"] != null)
                    {
                        //load Teardown items
                        var teardown = config["Teardown"].AsArray();
                        Teardown.AddRange(ParseItems(teardown));
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
        private List<TestItem> ParseItems(JsonArray array)
        {
            try
            {
                List<TestItem> list = new List<TestItem>();
                foreach (var item in array)
                {
                    var enumerator=item.AsObject().AsEnumerable().GetEnumerator();
                    var current=enumerator.Current;
                    enumerator.MoveNext();
                    current=enumerator.Current;
                    if (current.Key=="Script")
                    {
                        list.AddRange(ParseScriptsTestItems(item));
                    }
                    else if (current.Value.GetType()==typeof(JsonObject))
                    {
                        list.AddRange(ParseExecuterTestItems(item));
                    }
                    else
                    {

                    }
                }
                return list;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (Exception)
            {

                throw;
            }

        }
        private JsonNode ConvertToJsonNode(IConfiguration config)
        {
            try
            {
                var sb = new StringBuilder();
                JsonObject obj = new JsonObject();
                foreach (var child in config.GetChildren())
                {
                    if (child.Path.EndsWith("0"))
                    {
                        var arr = new JsonArray();
                        foreach (var arrayChild in config.GetChildren())
                        {
                            arr.Add(ConvertToJsonNode(arrayChild));
                        }

                        return arr;
                    }
                    else
                    {
                        obj.Add(child.Key, ConvertToJsonNode(child));
                    }
                }

                if (config is IConfigurationSection section && section.Value != null)
                {
                    var aa = section.Get<string>();
                    return JsonNode.Parse(section.Value);
                }


                return obj;
            }
            catch (JsonException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private IEnumerable<TestItem> ParseExecuterTestItems(JsonNode node)
        {
            if (node["Repeat"] != null)
            {
                var repeat = node["Repeat"].GetValue<int>();
                for (int i = 0; i < repeat; i++)
                {
                    yield return TestItem.Parse(node);
                }
            }
            else
            {
                yield return TestItem.Parse(node);
            }

        }

        private IEnumerable<TestItem> ParseScriptsTestItems(JsonNode item)
        {
            var p = item["Script"].GetValue<string>();
            List<TestItem> items = new List<TestItem>();
            if (item["Repeat"] != null)
            {
                var repeat = item["Repeat"].GetValue<int>();
                for (int i = 0; i < repeat; i++)
                {
                    items.AddRange(ReadFromScriptFile(p));
                }
            }
            else
            {
                items.AddRange(ReadFromScriptFile(p));
            }
            return items;
        }
        /// <summary>
        /// Read the file and convert to TestItem colletion
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>Collection of <see cref="TestItem"/></returns>
        private IEnumerable<TestItem> ReadFromScriptFile(string path)
        {
            JsonNode content;
            try
            {
                var p = Path.Combine(RootDirectory, path);
                content = JsonNode.Parse(File.ReadAllText(p));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            foreach (var item in content.AsArray())
            {
                var enumerator = item.AsObject().AsEnumerable().GetEnumerator();
                var current = enumerator.Current;
                enumerator.MoveNext();
                current = enumerator.Current;
                if (current.Key == "Script")
                {
                    foreach (var subitem in ParseScriptsTestItems(item))
                    {
                        yield return subitem;
                    }
                }
                else if (current.Value.GetType() == typeof(JsonObject))
                {
                    foreach (var subitem in ParseExecuterTestItems(item))
                    {
                        yield return subitem;
                    } ;
                }
                else
                {

                }

            }
        }
        private static IEnumerable<MethodInfo> GetExtensionMethods<T>(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(type => type.IsSealed && !type.IsGenericType && !type.IsNested);
            foreach (var type in types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (method.IsDefined(typeof(ExtensionAttribute), false) &&
                        method.GetParameters()[0].ParameterType == typeof(T))
                    {
                        yield return method;
                    }
                }
            }
        }
    }
}
