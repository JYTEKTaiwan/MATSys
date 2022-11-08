using CsvHelper.TypeConversion;
using Microsoft.Extensions.Configuration;
using System;
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
                    if (item.Contains(".ats"))
                    {
                        foreach (var subItem in ReadFromFile(item, type))
                        {
                            yield return subItem;
                        };
                    }
                    else
                    {
                        var pat = item.Split(':');
                        yield return new TestItem(type, pat[0], pat[1]);

                    }
                }
            }


        }
        private IEnumerable<TestItem> ReadFromFile(string filePath, ScriptType type)
        {
            var p = Path.Combine(RootDirectory, filePath);
            if (File.Exists(p))
            {
                foreach (var item in File.ReadLines(p))
                {
                    if (item.Contains(".ats"))
                    {
                        foreach (var subItem in ReadFromFile(item, type))
                        {
                            yield return subItem;
                        }
                    }
                    else
                    {
                        var pat = item.Split(':');
                        yield return new TestItem(type, pat[0], pat[1]);
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
