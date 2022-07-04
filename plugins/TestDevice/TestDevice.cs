using MATSys.Commands;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;

namespace TestDevice
{
    public class TestDevice : DeviceBase
    {
        public TestDevice(IServiceProvider services, string configurationKey) : base(services, configurationKey)
        {
        }

        public override void Load(IConfigurationSection section)
        {
        }

        public override void LoadFromObject(object configuration)
        {
        }
        public class Data
        {
            public string Date { get; set; } = "";
            public double Number { get; set; } = 0.0;
        }

        [Prototype("Test", typeof(Command<Data>))]
        public string Test(Data a)
        {
            Instance.Recorder.Write(a);
            Instance.Notifier.Publish(a);
            return a.Date + "---" + a.Number.ToString();
        }

        [Prototype("Method", typeof(Command<string>))]
        public string Method(string c)
        {
            return c;
        }

    }
}