using MATSys.Plugins;
using MATSys;
using MATSys.Commands;
using Microsoft.Extensions.Configuration;

namespace UT_MATSys;

public class UT_DeviceBase
{
    [Test]
    [Category("Initialize")]
    public void CreateFromInterface()
    {
        ITransceiver server = new EmptyTransceiver();
        INotifier bus = new EmptyNotifier();
        IRecorder recorder = new EmptyRecorder();
        IDevice dev = new NormalDevice(server, bus, recorder);
        Assert.IsTrue(dev != null);
    }
    [Test]
    [Category("Initialize")]
    public void CreateFromNullCommandServer()
    {
        ITransceiver server = null!;
        INotifier bus = new EmptyNotifier();
        IRecorder recorder = new EmptyRecorder();
        IDevice dev = new NormalDevice(server, bus, recorder);
        Assert.IsTrue(dev.Server.Name == nameof(EmptyTransceiver));
    }
    [Test]
    [Category("Initialize")]
    public void CreateFromNullDataBus()
    {
        ITransceiver server = new EmptyTransceiver();
        INotifier bus = null!;
        IRecorder recorder = new EmptyRecorder();
        IDevice dev = new NormalDevice(server, bus, recorder);
        Assert.IsTrue(dev.DataBus.Name == nameof(EmptyNotifier));
    }
    [Test]
    [Category("Initialize")]
    public void CreateFromNullDataRecorder()
    {
        ITransceiver server = new EmptyTransceiver();
        INotifier bus = new EmptyNotifier();
        IRecorder recorder = null!;
        IDevice dev = new NormalDevice(server, bus, recorder);
        Assert.IsTrue(dev.DataRecorder.Name == nameof(EmptyRecorder));
    }
    private class NormalDevice : DeviceBase
    {
        public NormalDevice(IServiceProvider services, string configurationKey) : base(services, configurationKey)
        {

        }
        public NormalDevice(ITransceiver server, INotifier bus, IRecorder recorder) : base(server, bus, recorder)
        {

        }

        public override void Load(IConfigurationSection section)
        {

        }

        [Prortotype("Hi", typeof(Command))]
        public string Hello()
        {
            return "WORLD";

        }

        [Prortotype("Exception", typeof(Command))]
        public void Exception()
        {

            throw new Exception("Exception");


        }
        [Prortotype("WrongArgs", typeof(Command<int>))]
        public void WrongArgs()
        {

            throw new Exception("WrongArgs");


        }

    }

    [Test]
    [Category("StartStop")]
    public void StopBeforeStart()
    {
        IDevice dev = new NormalDevice(null!, null!, null!);
        dev.StopService();
        Assert.IsTrue(!dev.IsRunning);
    }

    [Test]
    [Category("StartStop")]
    public void StartMultipleTimes()
    {
        var cts = new CancellationTokenSource();
        IDevice dev = new NormalDevice(null!, null!, null!);
        dev.StartService(cts.Token);
        dev.StartService(cts.Token);
        Assert.IsTrue(dev.IsRunning);
    }
    [Test]
    [Category("StartStop")]
    public void StopMultipleTimes()
    {
        var cts = new CancellationTokenSource();
        IDevice dev = new NormalDevice(null!, null!, null!);
        dev.StartService(cts.Token);
        dev.StopService();
        dev.StopService();
        Assert.IsTrue(!dev.IsRunning);
    }
    [Test]
    [Category("Execute")]
    public void ExecuteInString()
    {
        var cts = new CancellationTokenSource();
        IDevice dev = new NormalDevice(null!, null!, null!);
        dev.StartService(cts.Token);
        var res = dev.Execute(Newtonsoft.Json.JsonConvert.SerializeObject(CommandBase.Create("Hi")));
        dev.StopService();
        Assert.IsTrue(res == "WORLD");
    }
    [Test]
    [Category("Execute")]
    public void ExecuteInCommand()
    {
        var cts = new CancellationTokenSource();
        IDevice dev = new NormalDevice(null!, null!, null!);
        dev.StartService(cts.Token);
        var res = dev.Execute(CommandBase.Create("Hi"));
        dev.StopService();
        Assert.IsTrue(res == "WORLD");
    }
    [Test]
    [Category("Execute")]
    public void ExecuteWhenCommandNotFound()
    {
        var cts = new CancellationTokenSource();
        IDevice dev = new NormalDevice(null!, null!, null!);
        dev.StartService(cts.Token);
        var res = dev.Execute(CommandBase.Create("HO"));
        dev.StopService();
        Assert.IsTrue(res.Contains("NOTFOUND"));
    }
    [Test]
    [Category("Execute")]
    public void ExecuteWhenCommandNotFoundInString()
    {
        var cts = new CancellationTokenSource();
        IDevice dev = new NormalDevice(null!, null!, null!);
        dev.StartService(cts.Token);
        var res = dev.Execute(Newtonsoft.Json.JsonConvert.SerializeObject(CommandBase.Create("HO")));
        dev.StopService();
        Assert.IsTrue(res.Contains("NOTFOUND"));
    }
    [Test]
    [Category("Execute")]
    public void ExecuteThrowExceptionInMethod()
    {
        var cts = new CancellationTokenSource();
        IDevice dev = new NormalDevice(null!, null!, null!);
        dev.StartService(cts.Token);
        var res = dev.Execute(CommandBase.Create("Exception"));
        dev.StopService();
        Assert.IsTrue(res.Contains("EXEC_ERROR"));
    }
    [Test]
    [Category("Execute")]
    public void ExecuteWrongArguments()
    {
        var cts = new CancellationTokenSource();
        IDevice dev = new NormalDevice(null!, null!, null!);
        dev.StartService(cts.Token);
        var res = dev.Execute(CommandBase.Create("WrongArgs", 1));
        dev.StopService();
        Assert.IsTrue(res.Contains("EXEC_ERROR"));
    }

}

