using MATSys;
using MATSys.Commands;
using MATSys.Plugins;
using System.Net.WebSockets;

namespace UT_MATSys;

public class UT_ModuleBase
{
    [Test]
    [Category("Initialize")]
    public void ManuallyCreateWithoutParameters()
    {
        IModule dev = new NormalDevice();
        Assert.IsTrue(dev != null);
    }

    [Test]
    [Category("Initialize")]
    public void ManuallyCreateWithConfiguration()
    {
        IModule dev = new NormalDevice();
        Assert.IsTrue(dev != null);
    }


    [Test]
    [Category("Initialize")]
    public void ManuallyCreateWithTransceiver()
    {
        IModule dev = new NormalDevice();
        Assert.IsTrue(dev.Transceiver.Alias == nameof(EmptyTransceiver));
    }

    [Test]
    [Category("Initialize")]
    public void ManuallyCreateWithNotifier()
    {
        IModule dev = new NormalDevice();
        Assert.IsTrue(dev.Notifier.Alias == nameof(EmptyNotifier));
    }

    [Test]
    [Category("Initialize")]
    public void ManuallyCreateWithRecorder()
    {
        IModule dev = new NormalDevice();
        Assert.IsTrue(dev.Recorder.Alias == nameof(EmptyRecorder));
    }

    [Test]
    [Category("Execute")]
    public void ExecuteInString()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        dev.Execute(CommandBase.Create("Hi").Serialize(), out var res);
        Assert.IsTrue(res == System.Text.Json.JsonSerializer.Serialize("WORLD"));
    }

    [Test]
    [Category("Execute")]
    public void ExecuteInCommand()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        var res = dev.Execute(CommandBase.Create("Hi"));
        Assert.IsTrue(res == System.Text.Json.JsonSerializer.Serialize("WORLD"));
    }

    [Test]
    [Category("Execute")]
    public void ExecuteWhenCommandNotFound()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        var res = dev.Execute(CommandBase.Create("HO"));
        Assert.IsTrue(res.Contains(ExceptionHandler.cmd_notFound));
    }

    [Test]
    [Category("Execute")]
    public void ExecuteWhenCommandNotFoundInString()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        var res = dev.Execute(CommandBase.Create("HO").Serialize());
        Assert.IsTrue(res.Contains(ExceptionHandler.cmd_notFound));
    }

    [Test]
    [Category("Execute")]
    public void ExecuteThrowExceptionInMethod()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        var res = dev.Execute(CommandBase.Create("Exception"));
        Assert.IsTrue(res.Contains(ExceptionHandler.cmd_execError));
    }

    [Test]
    [Category("Execute")]
    public void ExecuteWrongArguments()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        var res = dev.Execute(CommandBase.Create("WrongArgs", 1.5).Serialize());
        Assert.IsTrue(res.Contains(ExceptionHandler.cmd_notFound));
    }
    [Test]
    [Category("Execute")]
    public void ExecuteSerDesError()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();

        dev.Execute(CommandBase.Create("WrongSerDes", 8.8).Serialize(), out var res);
        Assert.IsTrue(res.Contains(ExceptionHandler.cmd_execError));
    }
}