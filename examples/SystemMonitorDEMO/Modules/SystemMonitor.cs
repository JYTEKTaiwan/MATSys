using MATSys;
using MATSys.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SystemMonitorDEMO.Modules
{
    internal class SystemMonitor : ModuleBase
    {
        private Channel<PerformanceData> _ch = Channel.CreateBounded<PerformanceData>(new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropOldest });
        private CancellationTokenSource cts_pf = new CancellationTokenSource();
        private PerformanceCounter pf_processor = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
        private PerformanceCounter pf_memory = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        public SystemMonitor(object configuration, ITransceiver transceiver, INotifier notifier, IRecorder recorder, string aliasName = "") : base(configuration, transceiver, notifier, recorder, aliasName)

        {
        }

        public override void Load(IConfigurationSection section)
        {

        }

        public override void Load(object configuration)
        {

        }

        [MATSysCommandAttribute ("Start", typeof(Command))]
        public void StartMonitor()
        {
            cts_pf = new CancellationTokenSource();
            Task.Run(async () => 
            {
                while (!cts_pf.IsCancellationRequested)
                {
                    await _ch.Writer.WriteAsync(new PerformanceData(pf_processor.NextValue(), pf_memory.NextValue()));
                    await Task.Delay(250);                    
                }
            });
        }

        [MATSysCommandAttribute ("Stop", typeof(Command))]
        public void StopMonitor()
        {
            cts_pf.Cancel();
        }

        [MATSysCommandAttribute ("Machine",typeof(Command))]
        public string MachineName()
        {
            var str = Environment.MachineName;
            Base.Notifier.Publish(str);
            Base.Recorder.Write(str);
            return str;
        }

        [MATSysCommandAttribute ("ID", typeof(Command))]
        public string GetName()
        {
            Base.Notifier.Publish(Name);
            Base.Recorder.Write(Name);
            return Name;
        }

        [MATSysCommandAttribute ("Read",typeof(Command))]
        public string GetLatestData()
        {
             var result=_ch.Reader.ReadAsync().AsTask().Result;
            return $"{result.CPU.ToString("F6")}\t{result.Memory.ToString("F6")}";
        }

    }
    public struct PerformanceData
    {
        public float CPU { get; set; }
        public float Memory { get; set; }
        public PerformanceData(float cpu, float mem)
        {
            CPU = cpu;
            Memory = mem;
        }
    }
}
