using MATSys;
using MATSys.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitorDEMO.Modules
{
    internal class SystemMonitor : ModuleBase
    {
        public SystemMonitor(object configuration, ITransceiver transceiver, INotifier notifier, IRecorder recorder, string aliasName = "") : base(configuration, transceiver, notifier, recorder, aliasName)
        {
        }

        public override void Load(IConfigurationSection section)
        {

        }

        public override void Load(object configuration)
        {

        }

        [MethodName("Machine",typeof(Command))]
        public string MachineName()
        {
            var str = Environment.MachineName;
            Base.Notifier.Publish(str);
            Base.Recorder.Write(str);
            return str;
        }
        [MethodName("ID", typeof(Command))]
        public string GetName()
        {
            Base.Notifier.Publish(Name);
            Base.Recorder.Write(Name);
            return Name;
        }

    }
}
