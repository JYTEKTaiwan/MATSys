using MATSys;
using MATSys.Commands;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Management;
using System.Threading.Channels;

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
                    var data = new PerformanceData(pf_processor.NextValue(), (1 - pf_memory.NextValue() / _totalRAM) * 100.0);
                    await _ch.Writer.WriteAsync(data);
                    Base.Recorder.Write(data);
                    Base.Notifier.Publish(data);
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
            return str;
        }

        [MATSysCommand("ID")]
        public string GetName()
        {
            Base.Notifier.Publish(Name);
            return Name;
        }

        [MATSysCommand]
        public string GetLatestData()
        {
            var result = _ch.Reader.ReadAsync().AsTask().Result;

            return JsonConvert.SerializeObject(result);
        }

    }
    public struct PerformanceData
    {
        public double CPU { get; set; }
        public double Memory { get; set; }
        public PerformanceData(double cpu, double mem)
        {
            CPU = cpu;
            Memory = mem;
        }
    }
}
