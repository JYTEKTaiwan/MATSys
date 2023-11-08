// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using MATSys.Commands;

Console.WriteLine("Hello, World!");

var obj = new A();

CommandConverter.Convert(obj);
var sw = new Stopwatch();
sw.Restart();
for (int i = 0; i < 1000; i++)
{
    var a = CommandConverter.Convert(obj);

}
sw.Stop();

Console.WriteLine(sw.Elapsed);

Console.WriteLine("!");
Console.WriteLine();



[MATSysCommandContract("HI")]
public class A
{
}
