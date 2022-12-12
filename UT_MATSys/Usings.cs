global using NUnit.Framework;
using MATSys;
using MATSys.Commands;
using Microsoft.Extensions.Configuration;

namespace UT_MATSys;

public class NormalDevice : ModuleBase
{
    public NormalDevice(object? configuration = null, ITransceiver? transceiver = null, INotifier? notifier = null, IRecorder? recorder = null, string aliasName = "") : base(configuration, transceiver, notifier, recorder, aliasName)
    {
    }

    public override void Load(IConfigurationSection section)
    {
    }

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

    public override void Load(object configuration)
    {
    }
}

