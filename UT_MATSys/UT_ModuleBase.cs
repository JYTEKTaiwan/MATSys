using MATSys;
using MATSys.Commands;
using MATSys.Plugins;

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
        IModule dev = new NormalDevice(configuration: new object());
        Assert.IsTrue(dev != null);
    }


    [Test]
    [Category("Initialize")]
    public void ManuallyCreateWithTransceiver()
    {
        IModule dev = new NormalDevice(transceiver: new EmptyTransceiver());
        Assert.IsTrue(dev.Transceiver.Alias == nameof(EmptyTransceiver));
    }

    [Test]
    [Category("Initialize")]
    public void ManuallyCreateWithNotifier()
    {
        IModule dev = new NormalDevice(notifier: new EmptyNotifier());
        Assert.IsTrue(dev.Notifier.Alias == nameof(EmptyNotifier));
    }

    [Test]
    [Category("Initialize")]
    public void ManuallyCreateWithRecorder()
    {
        IModule dev = new NormalDevice(recorder: new EmptyRecorder());
        Assert.IsTrue(dev.Recorder.Alias == nameof(EmptyRecorder));
    }
    [Test]
    [Category("StartStop")]
    public void StopBeforeStart()
    {
        IModule dev = new NormalDevice();
        dev.StopPluginService();
        Assert.IsTrue(!dev.IsRunning);
    }

    [Test]
    [Category("StartStop")]
    public void StartMultipleTimes()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        dev.StartPluginService(cts.Token);
        dev.StartPluginService(cts.Token);
        Assert.IsTrue(dev.IsRunning);
    }

    [Test]
    [Category("StartStop")]
    public void StopMultipleTimes()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        dev.StartPluginService(cts.Token);
        dev.StopPluginService();
        dev.StopPluginService();
        Assert.IsTrue(!dev.IsRunning);
    }

    [Test]
    [Category("Execute")]
    public void ExecuteInString()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        dev.StartPluginService(cts.Token);
        var res = dev.Execute(CommandBase.Create("Hi").Serialize());
        dev.StopPluginService();
        Assert.IsTrue(res == System.Text.Json.JsonSerializer.Serialize("WORLD"));
    }

    [Test]
    [Category("Execute")]
    public void ExecuteInCommand()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        dev.StartPluginService(cts.Token);
        var res = dev.Execute(CommandBase.Create("Hi"));
        dev.StopPluginService();
        Assert.IsTrue(res == System.Text.Json.JsonSerializer.Serialize("WORLD"));
    }

    [Test]
    [Category("Execute")]
    public void ExecuteWhenCommandNotFound()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        dev.StartPluginService(cts.Token);
        var res = dev.Execute(CommandBase.Create("HO"));
        dev.StopPluginService();
        Assert.IsTrue(res.Contains(ExceptionHandler.cmd_notFound));
    }

    [Test]
    [Category("Execute")]
    public void ExecuteWhenCommandNotFoundInString()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice(null!, null!, null!, null!);
        dev.StartPluginService(cts.Token);
        var res = dev.Execute(CommandBase.Create("HO").Serialize());
        dev.StopPluginService();
        Assert.IsTrue(res.Contains(ExceptionHandler.cmd_notFound));
    }

    [Test]
    [Category("Execute")]
    public void ExecuteThrowExceptionInMethod()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        dev.StartPluginService(cts.Token);
        var res = dev.Execute(CommandBase.Create("Exception"));
        dev.StopPluginService();
        Assert.IsTrue(res.Contains(ExceptionHandler.cmd_execError));
    }

    [Test]
    [Category("Execute")]
    public void ExecuteWrongArguments()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        dev.StartPluginService(cts.Token);
        var res = dev.Execute(CommandBase.Create("WrongArgs", 1.5).Serialize());
        dev.StopPluginService();
        Assert.IsTrue(res.Contains(ExceptionHandler.cmd_serDesError));
    }
    [Test]
    [Category("Execute")]
    public void ExecuteSerDesError()
    {
        var cts = new CancellationTokenSource();
        IModule dev = new NormalDevice();
        dev.StartPluginService(cts.Token);

        var res = dev.Execute(CommandBase.Create("WrongSerDes", 8.8).Serialize());
        dev.StopPluginService();
        Assert.IsTrue(res.Contains(ExceptionHandler.cmd_serDesError));
    }
}