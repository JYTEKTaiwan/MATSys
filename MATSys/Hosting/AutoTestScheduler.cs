using CsvHelper.TypeConversion;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace MATSys.Hosting
{
    internal class AutoTestScheduler
    {
        private readonly Channel<TestItem> _queue = Channel.CreateUnbounded<TestItem>();
        private readonly TestScript script = new TestScript();
        public TestItemCollection SetupItems { get; } = new TestItemCollection();
        public TestItemCollection TestItems { get; } = new TestItemCollection();
        public TestItemCollection TeardownItems { get; } = new TestItemCollection();


        public bool IsAvailable => _queue.Reader.Count > 0;
        public AutoTestScheduler(IConfiguration config)
        {
            if (config.GetSection("MATSys:Scripts").Exists())
            {
                script = config.GetSection("MATSys:Scripts").Get<TestScript>();
                SetupItems.AddRange(script.ConvertToTestItem(script.Setup, ScriptType.Setup));
                TestItems.AddRange(script.ConvertToTestItem(script.Test, ScriptType.Test));
                TeardownItems.AddRange(script.ConvertToTestItem(script.Teardown, ScriptType.Teardown));
            }

        }
        public void AddSetupItem()
        {
            //setup
            foreach (var item in SetupItems)
            {
                _queue.Writer.WriteAsync(item).AsTask().Wait();
            }

        }
        public void AddTearDownItem()
        {
            //teardown
            foreach (var item in TeardownItems)
            {
                _queue.Writer.WriteAsync(item).AsTask().Wait();
            }
        }
        public void AddTestItem()
        {
            //test
            foreach (var item in TestItems)
            {
                _queue.Writer.WriteAsync(item).AsTask().Wait();
            }
        }
        public async Task<TestItem> Dequeue(CancellationToken token)
        {
            return await _queue.Reader.ReadAsync(token);
        }
    }
    internal class TestScript
    {
        public string RootDirectory { get; set; } = @".\scripts";
        public string[]? Setup { get; set; }
        public string[]? Test { get; set; }
        public string[]? Teardown { get; set; }
        public IEnumerable<TestItem> ConvertToTestItem(string[]? scripts, ScriptType type)
        {
            if (scripts != null)
            {
                foreach (var item in scripts)
                {
                    var obj = JsonObject.Parse(item).AsObject();
                    if (obj.ContainsKey ("Executer"))
                    {
                        var data = obj["Executer"].AsObject().First();
                        yield return new TestItem(type, data.Key, data.Value.ToJsonString());
                    }
                    else if (obj.ContainsKey( "Script"))
                    {
                        var path = obj["Script"].GetValue<string>();
                        var subitems = ReadFromFile(path, type);
                        foreach (var subItem in subitems)
                        {
                            yield return subItem;
                        }
                    }
                    else
                    {

                    }

                    //var items = ReadFromFile(item,type);
                    //foreach (var testItem in items)
                    //{
                    //    yield return testItem;
                    //}
                }
            }


        }
        private IEnumerable<TestItem> ReadFromFile(string filePath, ScriptType type)
        {
            var p = Path.Combine(RootDirectory, filePath);
            if (File.Exists(p))
            {
                var script = JsonObject.Parse(File.ReadAllText(p));
                foreach (var item in script.AsArray())
                {
                    var obj = item.AsObject().First();
                    if (obj.Key == "Executer")
                    {
                        var data = obj.Value.AsObject().First();
                        yield return new TestItem(type, data.Key, data.Value.ToJsonString());
                    }
                    else if (obj.Key == "Script")
                    {
                        var path = obj.Value.ToString();
                        var subitems= ReadFromFile(path, type);
                        foreach (var subItem in subitems)
                        {
                            yield return subItem;
                        }
                    }
                    else
                    {

                    }
                }

            }
        }

    }
    /// <summary>
    /// Type of the script
    /// </summary>
    public enum ScriptType
    {
        /// <summary>
        /// setup script
        /// </summary>
        Setup,
        /// <summary>
        /// test script
        /// </summary>
        Test,
        /// <summary>
        /// teardown script
        /// </summary>
        Teardown
    }

    /// <summary>
    /// Represent the single test item in the script mode
    /// </summary>
    public class TestItem
    {
        /// <summary>
        /// Type of the test item
        /// </summary>
        public ScriptType Type { get; set; }
        /// <summary>
        /// Name of the module
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Command string 
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Unique ID for the instance
        /// </summary>
        public int ID { get; } = -1;

        /// <summary>
        /// Constructor for TestItem
        /// </summary>
        /// <param name="type">type of the script</param>
        /// <param name="name">Module name</param>
        /// <param name="cmd">Command string</param>
        public TestItem(ScriptType type, string name, string cmd)
        {
            Type = type;
            ID = GetHashCode();
            ModuleName = name;
            Command = cmd;
        }

    }

    /// <summary>
    /// Collection of test item
    /// </summary>
    public class TestItemCollection : List<TestItem>
    {
        /// <summary>
        /// Return test item using its unique ID property as key
        /// </summary>
        /// <param name="id">the unique id of the test item</param>
        /// <returns>test item</returns>
        public new TestItem? this[int id] => this.FirstOrDefault(x => x.ID == id);
    }

}
