// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using MATSys.Commands;

Console.WriteLine("Hello, World!");

var obj = new A();

var b=CommandConverter.Convert(obj);
Console.WriteLine(b.Serialize());
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
    
    [MATSysCommandOrder(20)]
    public DateTime Dt{get;set;}=DateTime.Now;
    [MATSysCommandOrder(0)]
    public int Number{get;set;}
    [MATSysCommandOrder(-1)]
    public CustomData Float{get;set;}=new CustomData();
}

public class CustomData
{
    public int A{get;set;}=-1;
    public bool  B{get;set;}=true;
}
