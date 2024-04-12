global using NUnit.Framework;
using MATSys;
using MATSys.Commands;

namespace UT_MATSys;

public class NormalDevice : ModuleBase
{
    public override object Configuration { get; set; }
    [MATSysCommand("Hi")]
    public string Hello()
    {
        return "WORLD";
    }

    [MATSysCommand("Exception")]
    public void Exception()
    {
        throw new Exception();
    }

    [MATSysCommand("WrongArgs")]
    public void WrongArgs()
    {

    }

    [MATSysCommand("WrongSerDes")]
    public void WrongSerDes(int i)
    {
    }

}

