using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MATSys.Hosting
{
    internal class AutoTestScheduler
    {
        private readonly Channel<TestItem> _queue = Channel.CreateUnbounded<TestItem>();
        private readonly TestScript script = new TestScript();
        private List<TestItem> testItems = new List<TestItem>();
        public bool IsAvailable => _queue.Reader.Count > 0;
        public AutoTestScheduler(IConfiguration config)
        {
            if (config.GetSection("MATSys:Scripts").Exists())
            {
                script = config.GetSection("MATSys:Scripts").Get<TestScript>();
                testItems.AddRange(TestScript.GetTestItems(script.Setup));
                testItems.AddRange(TestScript.GetTestItems(script.Test));
                testItems.AddRange(TestScript.GetTestItems(script.Teardown));
            }

        }
        public void RunSetup()
        {
            //setup
            foreach (var item in TestScript.GetTestItems(script.Setup))
            {
                _queue.Writer.WriteAsync(item).AsTask().Wait(100);
            }

        }
        public void RunTeardown()
        {
            //teardown
            foreach (var item in TestScript.GetTestItems(script.Teardown))
            {
                _queue.Writer.WriteAsync(item).AsTask().Wait(100);
            }
        }

        public void RunTestItem()
        {
            //test
            foreach (var item in TestScript.GetTestItems(script.Test))
            {
                _queue.Writer.WriteAsync(item).AsTask().Wait(100);
            }
        }


        public async Task<TestItem> Dequeue(CancellationToken token)
        {
            return await _queue.Reader.ReadAsync(token);
        }

    }
    internal class TestScript
    {
        public string[]? Setup { get; set; }
        public string[]? Test { get; set; }
        public string[]? Teardown { get; set; }
        public static IEnumerable<TestItem> GetTestItems(string[]? scripts)
        {
            if (scripts != null)
            {
                foreach (var item in scripts)
                {
                    var pat = item.Split(':');
                    yield return new TestItem(pat[0], pat[1]);
                }
            }

        }

    }

    public struct TestItem
    {
        public string ModuleName { get; set; }
        public string Command { get; set; }
        public TestItem(string name, string cmd)
        {
            ModuleName = name;
            Command = cmd;
        }
        public static TestItem Empty => new TestItem("", "");

    }

}
