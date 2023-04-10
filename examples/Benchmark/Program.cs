// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using MATSys;
using MATSys.Commands;
using MATSys.Factories;
using MATSys.Hosting;
using MATSys.Hosting.Scripting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Benchmark
{
    internal class Benchmark
    {
        static void Main()
        {
            BenchmarkRunner.Run<MATSysInstance>();
        }
    }
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net472)]
    [MemoryDiagnoser]
    public class MATSysInstance
    {
        private IModule mod;
        private string cmd0 = CommandBase.Create("NoArgument").Serialize();
        private string cmd1 = CommandBase.Create("OneArgument", "HI").Serialize();
        private string cmd2 = CommandBase.Create("TwoArgument", 1, 1.0).Serialize();
        private string cmd3 = CommandBase.Create("ThreeArgument", 1, 1.0, "HI").Serialize();
        private string cmd4 = CommandBase.Create("FourArgument", 1, 1.0, "HI", true).Serialize();
        private string cmd5 = CommandBase.Create("FiveArgument", 1, 1.0, "HI", true, DateTime.Now).Serialize();
        private string cmd6 = CommandBase.Create("SixArgument", 1, 1.0, "HI", true, DateTime.Now, new Object()).Serialize();
        private string cmd7 = CommandBase.Create("SevenArgument", 1, 1.0, "HI", true, DateTime.Now, new Object(), (decimal)10).Serialize();

        public MATSysInstance() => mod = ModuleFactory.CreateNew<TestDevice>(null);

        [Benchmark]
        public void Arg0Command()
        {
            mod.Execute(cmd0);
        }

        [Benchmark]
        public void Arg1Command()
        {
            mod.Execute(cmd1);
        }
        [Benchmark]
        public void Arg2Command()
        {
            mod.Execute(cmd2);

        }
        [Benchmark]
        public void Arg3Command()
        {
            mod.Execute(cmd3);

        }
        [Benchmark]
        public void Arg4Command()
        {
            mod.Execute(cmd4);

        }
        [Benchmark]
        public void Arg5Command()
        {
            mod.Execute(cmd5);

        }
        [Benchmark]
        public void Arg6Command()
        {
            mod.Execute(cmd6);

        }

        [Benchmark]
        public void Arg7Command()
        {
            mod.Execute(cmd7);

        }

    }

    public class TestDevice : ModuleBase
    {


        public class Data
        {
            public string Date { get; set; } = "";
            public double Number { get; set; } = 0.0;
        }
        [MATSysCommand]
        public string NoArgument()
        {
            return "0";
        }
        [MATSysCommand]
        public string OneArgument(string a)
        {
            return "1";
        }
        [MATSysCommand]
        public string TwoArgument(int a, double b)
        {
            return "2";
        }
        [MATSysCommand]
        public string ThreeArgument(int a, double b, string c)
        {
            return "3";
        }
        [MATSysCommand]
        public string FourArgument(int a, double b, string c, bool d)
        {
            return "4";
        }
        [MATSysCommand]
        public string FiveArgument(int a, double b, string c, bool d, DateTime e)
        {
            return "5";
        }
        [MATSysCommand]
        public string SixArgument(int a, double b, string c, bool d, DateTime e, object f)
        {
            return "6";
        }

        [MATSysCommand]
        public string SevenArgument(int a, double b, string c, bool d, DateTime e, object f, decimal g)
        {
            return "7";
        }


    }

}





