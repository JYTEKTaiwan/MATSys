// See https://aka.ms/new-console-template for more information
using MATSys.Hosting.Grpc;
using System.Diagnostics;
using System.Reflection;
using MATSys.Commands;

Console.WriteLine("Hello, World!");

var obj = new B(2.4,1 );
var obj2 = new C(5,9.6);
var b = CommandConverter.Convert(new A());
Console.WriteLine(b.Serialize());
b = CommandConverter.Convert(obj);
Console.WriteLine(b.Serialize());
b = CommandConverter.Convert(obj2);
Console.WriteLine(b.Serialize());

//var sw = new Stopwatch();
//sw.Restart();
//for (int i = 0; i < 1000; i++)
//{
//    var a = CommandConverter.Convert(obj);

//}
//sw.Stop();

//Console.WriteLine(sw.Elapsed);

Console.WriteLine("!");
Console.WriteLine();

[MATSysCommandContract("AC")]
public record C(int v3,double v1);

[MATSysCommandContract("A")]
public record B
{

    [MATSysCommandOrder(-6)]
    public double V2 { get; set; }
    [MATSysCommandOrder(-1)]
    public int V35 { get; set; }
    public B(double v2, int v35)
    {
        V35 = v35;
        V2 = v2;
    }
}


[MATSysCommandContract("HI")]
public class A
{

    [MATSysCommandOrder(20)]
    public DateTime Dt { get; set; } = DateTime.Now;
    [MATSysCommandOrder(0)]
    public int Number { get; set; }
    [MATSysCommandOrder(100)]
    public CustomData Float { get; set; } = new CustomData();
}

public class CustomData
{
    public int A { get; set; } = -1;
    public bool B { get; set; } = true;
}
