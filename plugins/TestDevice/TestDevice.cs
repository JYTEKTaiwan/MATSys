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

        public override void Load(IConfigurationSection section)
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
    }
}