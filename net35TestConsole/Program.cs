using MATSys;
using MATSys.Commands;
using System;
using System.Threading;

namespace net35TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cmd = CommandBase.Create("Delay", 1000);

            var mod = new TestDevice();
            mod.Configure(null);

            var aa = mod.Execute(cmd);

            Console.WriteLine(aa);

            var o = MATSys.Utilities.Serializer.Deserialize(aa, cmd.GetType());
            Console.ReadKey();

        }
    }

    internal class TestDevice : ModuleBase
    {
        [MATSysCommand("Delay")]
        public void Delay(int a)
        {
            Console.WriteLine($"[{DateTime.Now}]Start - {a}");
            Thread.Sleep(a);
            Console.WriteLine($"[{DateTime.Now}]Done - {a}");
        }


    }

}