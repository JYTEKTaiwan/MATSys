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
        ICommandServer server = new EmptyCommandServer();
        IDataBus bus = new EmptyDataBus();
        IRecorder recorder = new EmptyRecorder();
        IDevice dev = new NormalDevice(server, bus, recorder);
        Assert.IsTrue(dev != null);
    }
    [Test]
    [Category("Initialize")]
    public void CreateFromNullCommandServer()
    {
        ICommandServer server = null!;
        IDataBus bus = new EmptyDataBus();
        IRecorder recorder = new EmptyRecorder();
        IDevice dev = new NormalDevice(server, bus, recorder);
        Assert.IsTrue(dev.Server.Name == nameof(EmptyCommandServer));
    }
    [Test]
    [Category("Initialize")]
    public void CreateFromNullDataBus()
    {
        ICommandServer server = new EmptyCommandServer();
        IDataBus bus = null!;
        IRecorder recorder = new EmptyRecorder();
        IDevice dev = new NormalDevice(server, bus, recorder);
        Assert.IsTrue(dev.DataBus.Name == nameof(EmptyDataBus));
    }
    [Test]
    [Category("Initialize")]
    public void CreateFromNullDataRecorder()
    {
        ICommandServer server = new EmptyCommandServer();
        IDataBus bus = new EmptyDataBus();
        IRecorder recorder = null!;
        IDevice dev = new NormalDevice(server, bus, recorder);
        Assert.IsTrue(dev.DataRecorder.Name == nameof(EmptyRecorder));
    }
    private class NormalDevice : DeviceBase
    {
        public NormalDevice(IServiceProvider services, string configurationKey) : base(services, configurationKey)
        {

        }
        public NormalDevice(ICommandServer server, IDataBus bus, IRecorder recorder) : base(server, bus, recorder)
        {

        }

        public override void Load(IConfigurationSection section)
        {

        }

        [MATSysCommand("Hi", typeof(Command))]
        public string Hello()
        {
            return "WORLD";

        }

        [MATSysCommand("Exception", typeof(Command))]
        public void Exception()
        {

            throw new Exception("Exception");


        }
        [MATSysCommand("WrongArgs", typeof(Command<int>))]
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

