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

            //var aa = mod.ExecuteCommandString("{\"Delay\":[1000]}");

            var aa = mod.Execute(CommandConverter.Convert(new CMD() { delay = 3000 }));
            
            Console.WriteLine(aa);

            
            Console.ReadKey();

        }
    }

    [MATSysCommandContract("Delay")]
    public class CMD
    {
        [MATSysCommandOrder(0)]
        public int delay { get; set; }
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