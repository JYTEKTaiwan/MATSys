using MATSys;
using MATSys.Commands;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SystemMonitorDEMO.Modules
{
    internal class SystemMonitor : ModuleBase
    {
        private Channel<PerformanceData> _ch = Channel.CreateBounded<PerformanceData>(new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropOldest });
        private CancellationTokenSource cts_pf = new CancellationTokenSource();
        private PerformanceCounter pf_processor = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private PerformanceCounter pf_memory = new PerformanceCounter("Memory", "Available Bytes");
        private float _totalRAM = 0;
        
        
        public SystemMonitor(object configuration, ITransceiver transceiver, INotifier notifier, IRecorder recorder, string aliasName = "") : base(configuration, transceiver, notifier, recorder, aliasName)
        {
            ManagementObjectSearcher Search = new ManagementObjectSearcher();
            Search.Query = new ObjectQuery("Select * From Win32_ComputerSystem");
            foreach (var item in Search.Get())
            {
                _totalRAM = Convert.ToSingle(item["TotalPhysicalMemory"]);
                if (_totalRAM == null)
                    break;
            }

        }

        public override void Load(IConfigurationSection section)
        {

        }

        public override void Load(object configuration)
        {

        }

        [MATSysCommand]
        public void StartMonitor()
        {
            cts_pf = new CancellationTokenSource();
            Task.Run(async () => 
            {
                while (!cts_pf.IsCancellationRequested)
                {
                    await _ch.Writer.WriteAsync(new PerformanceData(pf_processor.NextValue(), 1-pf_memory.NextValue()/ _totalRAM));
                    await Task.Delay(250);                    
                }
            });
        }

        [MATSysCommand]
        public void StopMonitor()
        {
            cts_pf.Cancel();
        }

        [MATSysCommand("Machine")]
        public string MachineName()
        {
            var str = Environment.MachineName;
            Base.Notifier.Publish(str);
            Base.Recorder.Write(str);
            return str;
        }

        [MATSysCommand("ID")]
        public string GetName()
        {
            Base.Notifier.Publish(Name);
            Base.Recorder.Write(Name);
            return Name;
        }

        [MATSysCommand]
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
