

using BenchmarkDotNet.Attributes;

/* Unmerged change from project 'InvokerBenchmark (net8.0)'
Before:
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
After:
using BenchmarkDotNet.Running;
using Microsoft.CodeAnalysis.CSharp.Syntax;
*/

/* Unmerged change from project 'InvokerBenchmark (net6.0)'
Before:
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
After:
using BenchmarkDotNet.Running;
using Microsoft.CodeAnalysis.CSharp.Syntax;
*/
using BenchmarkDotNet.Running;
using System.Reflection;
using System.Runtime.CompilerServices;

BenchmarkRunner.Run<InvokerBenchmark>();


[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80, baseline: true)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net70)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net60)]
public class InvokerBenchmark
{

    private Data data = new Data();
    private const string name = "test";
    private MethodInfo mi;
    private Func<string, string> func;
#if NET8_0_OR_GREATER
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Hello")]
    extern static string SayHello(Data @this, string name);
#endif
    [GlobalSetup]
    public void Init()
    {
        mi = data.GetType().GetMethod("Hello");
        func = System.Delegate.CreateDelegate(typeof(Func<string, string>), data, "Hello") as Func<string, string>;
        Direct();
        Reflection();
        Delegate();
        UnsafeAccessor();
    }
    [Benchmark]
    public void Direct()//4.5498ns
    {
        data.Hello(name);
    }
    [Benchmark]
    public void Reflection()//35.2718ns
    {
        mi.Invoke(data, new object[] { name });

    }
    [Benchmark]
    public void Delegate()//4.9910 ns (10% overhead)
    {
        func.Invoke(name);
    }
    [Benchmark]
    public void UnsafeAccessor()//4.5958ns (1% overhead)
    {
#if NET8_0_OR_GREATER
        SayHello(data, name);
#endif
    }

    internal class Data
    {
        public string Hello(string name)
        {
            return $"World:{name}";
        }
    }
}
