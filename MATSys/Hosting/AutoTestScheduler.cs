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
                testItems.AddRange(TestScript.GetTestItems(script.PreTest));
                testItems.AddRange(TestScript.GetTestItems(script.Test));
                testItems.AddRange(TestScript.GetTestItems(script.Result));
                testItems.AddRange(TestScript.GetTestItems(script.PostTest));
            }

        }
        public  void SingleTest()
        {
            foreach (var item in testItems)
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
        public string[] PreTest { get; set; }
        public string[] Test { get; set; }
        public string[] Result { get; set; }
        public string[] PostTest { get; set; }
        public static IEnumerable<TestItem> GetTestItems(string[] scripts)
        {
            foreach (var item in scripts)
            {
                var pat = item.Split(':');
                yield return new TestItem(pat[0], pat[1]);
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
