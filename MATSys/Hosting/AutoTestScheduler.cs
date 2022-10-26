﻿using Microsoft.Extensions.Configuration;
using System.Threading.Channels;

namespace MATSys.Hosting
{
    internal class AutoTestScheduler
    {
        private readonly Channel<TestItem> _queue = Channel.CreateUnbounded<TestItem>();
        private readonly TestScript script = new TestScript();
        private List<TestItem> setupItems = new List<TestItem>();
        private List<TestItem> testItems = new List<TestItem>();
        private List<TestItem> teardownItems = new List<TestItem>();

        public bool IsAvailable => _queue.Reader.Count > 0;
        public AutoTestScheduler(IConfiguration config)
        {
            if (config.GetSection("MATSys:Scripts").Exists())
            {
                script = config.GetSection("MATSys:Scripts").Get<TestScript>();
                setupItems.AddRange(script.ConvertToTestItem(script.Setup));
                testItems.AddRange(script.ConvertToTestItem(script.Test));
                teardownItems.AddRange(script.ConvertToTestItem(script.Teardown));
            }

        }
        public void AddSetupItem()
        {
            //setup
            foreach (var item in setupItems)
            {
                _queue.Writer.WriteAsync(item).AsTask().Wait(1);
            }

        }
        public void AddTearDownItem()
        {
            //teardown
            foreach (var item in teardownItems)
            {
                _queue.Writer.WriteAsync(item).AsTask().Wait(1);
            }
        }

        public void AddTestItem()
        {
            //test
            foreach (var item in testItems)
            {
                _queue.Writer.WriteAsync(item).AsTask().Wait(1);
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
        public IEnumerable<TestItem> ConvertToTestItem(string[]? scripts)
        {
            if (scripts != null)
            {
                foreach (var item in scripts)
                {
                    if (item.Contains(".ats"))
                    {
                        foreach (var subItem in ReadFromFile(item))
                        {
                            yield return subItem;
                        } ;
                    }
                    else
                    {
                        var pat = item.Split(':');
                        yield return new TestItem(pat[0], pat[1]);

                    }
                }
            }


        }
        private IEnumerable<TestItem> ReadFromFile(string filePath)
        {
            var p = Path.Combine(RootDirectory, filePath);
            if (File.Exists(p))
            {
                foreach (var item in File.ReadLines(p))
                {
                    if (item.Contains(".ats"))
                    {
                        foreach (var subItem in ReadFromFile(item))
                        {
                            yield return subItem;
                        }
                    }
                    else
                    {
                        var pat = item.Split(':');
                        yield return new TestItem(pat[0],pat[1]);
                    }

                }
            }
        }

    }

    /// <summary>
    /// Represent the single test item in the script mode
    /// </summary>
    public struct TestItem
    {
        /// <summary>
        /// Name of the module
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Command string 
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Constructor for TestItem
        /// </summary>
        /// <param name="name">Module name</param>
        /// <param name="cmd">Command string</param>
        public TestItem(string name, string cmd)
        {
            ModuleName = name;
            Command = cmd;
        }
        public static TestItem Empty => new TestItem("", "");

    }

}
