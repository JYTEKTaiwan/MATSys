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
        private readonly List<ITestItem> testItems=new List<ITestItem>();
        private readonly Channel<ITestItem> _queue =  Channel.CreateUnbounded<ITestItem>();

        public bool IsAvailable => _queue.Reader.Count > 0;
        public AutoTestScheduler()
        {
            testItems.Add(new PreTestItem());
            testItems.Add(new TestItem());
            testItems.Add(new UploadTestItem());
            testItems.Add(new PostTestItem());
        }
        public  void SingleTest()
        {
            foreach (var item in testItems)
            {
                _queue.Writer.WriteAsync(item).AsTask().Wait(100);
            }            
        }
        public async Task<ITestItem> Dequeue(CancellationToken token)
        {
            return await _queue.Reader.ReadAsync(token);
        }

    }

    public interface ITestItem
    {
        void Execute();

    }
    public class PreTestItem : ITestItem
    {
        public void Execute()
        {
            Console.WriteLine("I'm PreTest");

        }

    }
    public class TestItem : ITestItem
    {
        public void Execute()
        {
            Console.WriteLine("I'm Test");

        }

    }
    public class UploadTestItem:ITestItem
    {
        public void Execute()
        {
            Console.WriteLine("I'm Upload");

        }
    }
    public class PostTestItem : ITestItem
    {
        public void Execute()
        {
            Console.WriteLine("I'm PostTest");

        }

    }
}
