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
            
            var mod = new TestDevice();
            
            var aa = mod.Execute("Delay",1000);

            Console.WriteLine(aa);

            
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