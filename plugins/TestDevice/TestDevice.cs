using MATSys.Commands;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;

namespace MATSys.Devices
{
    public class TestDevice : DeviceBase
    {
        public TestDevice(IServiceProvider services, string configurationKey) : base(services, configurationKey)
        {
        }

        public TestDevice(ICommandServer server, IDataBus bus, IDataRecorder recorder) : base(server, bus, recorder)
        {
        }

        [CommandObject("MethodTest", typeof(Command))]
        public void Test()
        {
        }

        [CommandObject("MethodA", typeof(Command<int>))]
        public void A(int a)
        {
        }

        public override void Load(Microsoft.Extensions.Configuration.IConfigurationSection section)
        {
            throw new NotImplementedException();
        }
    }
}