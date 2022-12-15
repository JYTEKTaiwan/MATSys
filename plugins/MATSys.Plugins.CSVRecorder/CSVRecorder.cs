using CsvHelper;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace MATSys.Plugins
{
    /// <summary>
    /// Recorder implemented by CsvHelper library
    /// </summary>
    public class CSVRecorder : IRecorder
    {
        
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();
        private Channel<object>? _queue;
        private CSVRecorderConfiguration? _config;
        private CancellationTokenSource _localCts = new CancellationTokenSource();
        private Task _task = Task.CompletedTask;

        public string Name => nameof(CSVRecorder);

        public static CSVRecorder Create(CSVRecorderConfiguration option)
        {
            var recorder = new CSVRecorder();
            recorder.Load(option);
            return recorder;
        }
        public void Load(IConfigurationSection section)
        {
            _config = section.Get<CSVRecorderConfiguration>() ?? CSVRecorderConfiguration.Default;
            _logger = _config.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger(); ;

            _queue = Channel.CreateBounded<object>(new BoundedChannelOptions(_config.QueueSize) { FullMode = _config.BoundedChannelFullMode });
            _logger.Info("CSVRecorder initiated");
        }

        public void Load(object configuration)
        {
            _config = (configuration as CSVRecorderConfiguration) ?? CSVRecorderConfiguration.Default;
            _logger = _config.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger(); ;

            _queue = Channel.CreateBounded<object>(new BoundedChannelOptions(_config.QueueSize) { FullMode = _config.BoundedChannelFullMode });
            _logger.Info("CSVRecorder initiated");
        }

        public void Write(object data)
        {
            WriteAsync(data).Wait();
        }

        public void StopService()
        {
            if (_config!.WaitForComplete)
            {
                SpinWait.SpinUntil(() =>
                {
                    return _queue!.Reader.Count == 0;
                }, _config.WaitForCompleteTimeout);
            }
            _localCts.Cancel();
            _logger.Info("Stop service");
        }

        private bool TaskStatusCheck(Task t)
        {
            return t.Status == TaskStatus.RanToCompletion || t.Status == TaskStatus.Canceled || t.Status == TaskStatus.Faulted;
        }

        public void StartService(CancellationToken token)
        {
            try
            {
                _logger.Trace("Prepare to run");
                _localCts = new CancellationTokenSource();
                string baseFolder = AppDomain.CurrentDomain.BaseDirectory;
                var filename = Path.Combine(baseFolder, DateTime.Now.ToString("HHmmssfff") + ".csv");
                var _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
                object? data;
                StreamWriter streamWriter = new StreamWriter(filename);
                CsvWriter writer = new CsvWriter(streamWriter, System.Globalization.CultureInfo.InvariantCulture);
                Task.Run(() =>
                {
                    while (!_linkedCts.IsCancellationRequested)
                    {
                        if (_queue!.Reader.TryRead(out data))
                        {
                            writer.WriteRecord(data);
                            writer.NextRecord();
                            writer.Flush();
                            streamWriter.Flush();
                            _logger.Debug($"Data is written to file, elements in queue:{_queue.Reader.Count}");
                        }
                        SpinWait.SpinUntil(() => token.IsCancellationRequested, 1);
                    }
                    _queue!.Writer.Complete();
                    streamWriter.Close();
                    _logger.Debug("File stream and queue are closed");
                    _localCts.Dispose();
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task WriteAsync(object data)
        {
            await Task.Run(() =>
            {
                _logger.Debug(JsonSerializer.Serialize(data));
                _queue!.Writer.TryWrite(data);
                _logger.Info("Data is queued to filestream");
            });
        }
        public JsonObject Export()
        {
            return JsonObject.Parse(JsonSerializer.Serialize(_config)).AsObject();
        }
        public string Export(bool indented = true)
        {
            return Export().ToJsonString(new JsonSerializerOptions() { WriteIndented = indented });
        }
        
    }
    /// <summary>
    /// Configuration definition for CSVRecorder
    /// </summary>
    public class CSVRecorderConfiguration : IMATSysConfiguration
    {
        public string Type { get; set; } = "csv";
        public bool EnableLogging { get; set; } = false;
        public bool WaitForComplete { get; set; } = true;
        public int QueueSize { get; set; } = 2000;
        public int WaitForCompleteTimeout { get; set; } = 5000;
        public static CSVRecorderConfiguration Default = new CSVRecorderConfiguration();

        public BoundedChannelFullMode BoundedChannelFullMode { get; set; } = BoundedChannelFullMode.DropOldest;
    }
}