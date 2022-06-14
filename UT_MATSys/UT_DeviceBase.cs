using MATSys.Plugins;
using MATSys;
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
        IDataRecorder recorder = new EmptyDataRecorder();
        IDevice dev = new Device(server, bus, recorder);
        Assert.IsTrue(dev != null);
    }
    [Test]
    [Category("Initialize")]
    public void CreateFromNullCommandServer()
    {
        ICommandServer server = null;
        IDataBus bus = new EmptyDataBus();
        IDataRecorder recorder = new EmptyDataRecorder();
        IDevice dev = new Device(server, bus, recorder);
        Assert.IsTrue(dev.Server.Name==nameof(EmptyCommandServer));
    }
        [Test]
    [Category("Initialize")]
    public void CreateFromNullDataBus()
    {
        ICommandServer server = new EmptyCommandServer();
        IDataBus bus = null;
        IDataRecorder recorder = new EmptyDataRecorder();
        IDevice dev = new Device(server, bus, recorder);
        Assert.IsTrue(dev.Server.Name==nameof(EmptyDataBus));
    }
        [Test]
    [Category("Initialize")]
    public void CreateFromNullDataRecorder()
    {
        ICommandServer server = new EmptyCommandServer();
        IDataBus bus = new EmptyDataBus();
        IDataRecorder recorder = null;
        IDevice dev = new Device(server, bus, recorder);
        Assert.IsTrue(dev.Server.Name==nameof(EmptyDataRecorder));
    }
    private class Device : DeviceBase
    {
        public Device(IServiceProvider services, string configurationKey) : base(services, configurationKey)
        {

        }
        public Device(ICommandServer server, IDataBus bus, IDataRecorder recorder) : base(server, bus, recorder)
        {

        }

        public override void Load(IConfigurationSection section)
        {

        }

    }
}
