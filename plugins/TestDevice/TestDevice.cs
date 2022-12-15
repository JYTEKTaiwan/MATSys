using MATSys;
using MATSys.Commands;
using Microsoft.Extensions.Configuration;

namespace MATSys.Modules
{
    public class TestDevice : ModuleBase
    {
        public TestDevice(object configuration, ITransceiver server, INotifier bus, IRecorder recorder, string configurationKey = "") : base(configuration, server, bus, recorder, configurationKey)
        {
        }

        public override void Load(IConfigurationSection section)
        {
        }

        public override void Load(object configuration)
        {
        }

        public class Data
        {
            public string Date { get; set; } = "";
            public double Number { get; set; } = 0.0;
        }

        [MATSysCommand("Test", typeof(Command<Data>))]
        public string Test(Data a)
        {
            Base.Recorder.Write(a);
            Base.Notifier.Publish(a);
            return a.Date + "---" + a.Number.ToString();
        }

        [MATSysCommand]
        public string Method(string c)
        {
            return c;
        }

        [MATSysCommand]
        public string Number(string c, int i, double x, Data j, bool b)
        {
            return c;
        }
    }
}