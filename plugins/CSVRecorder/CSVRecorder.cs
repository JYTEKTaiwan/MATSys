using CsvHelper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Channels;

namespace MATSys.Plugins
{
    public class CSVRecorder<T> : IRecorder<T> where T : class
    {
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
        private Channel<T>? _queue;
        private CSVRecorderConfiguration? _config;

        private CancellationTokenSource _localCts = new CancellationTokenSource();
        private Task _task = Task.CompletedTask;

        public string Name => nameof(CSVRecorder);

        public void Load(IConfigurationSection section)
        {
            _config = section.Get<CSVRecorderConfiguration>() ?? CSVRecorderConfiguration.Default;
            _queue = Channel.CreateBounded<T>(new BoundedChannelOptions(_config.QueueSize) { FullMode = _config.BoundedChannelFullMode });
            _logger.Info("CSVRecorder initiated");
        }
        
        public void LoadFromObject(object configuration)
        {
            _config = (configuration as CSVRecorderConfiguration) ?? CSVRecorderConfiguration.Default;
            _queue = Channel.CreateBounded<T>(new BoundedChannelOptions(_config.QueueSize) { FullMode = _config.BoundedChannelFullMode });
            _logger.Info("CSVRecorder initiated");
        }

        public void Write(T data)
        {
            WriteAsync(data).Wait();
        }

        public void StopService()
        {
            _localCts.Cancel();
            _logger.Info("Stop service");
        }

        private bool TaskStatusCheck(Task t)
        {
            return t.Status == TaskStatus.RanToCompletion || t.Status == TaskStatus.Canceled || t.Status == TaskStatus.Faulted;
        }

        public void StartService(CancellationToken token)
        {
            _logger.Trace("Prepare to run");
            _localCts = new CancellationTokenSource();
            var filename = DateTime.Now.ToString("HHmmssfff") + ".csv";
            _queue = Channel.CreateBounded<T>(new BoundedChannelOptions(2000) { FullMode = BoundedChannelFullMode.DropOldest });
            var _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
            T? data;
            StreamWriter streamWriter = new StreamWriter(filename);
            CsvWriter writer = new CsvWriter(streamWriter, System.Globalization.CultureInfo.InvariantCulture);
            writer.WriteHeader<T>();
            writer.NextRecord();

            Task.Run(() =>
            {
                //errors happened in the internal loop were clarified as fatal error 
                try
                {                  
                    while (!_linkedCts.IsCancellationRequested)
                    {
                        if (_queue.Reader.TryRead(out data))
                        {
                            writer.WriteRecord(data);
                            writer.NextRecord();
                            _logger.Debug($"Data is written to file, elements in queue:{_queue.Reader.Count}");
                        }
                        SpinWait.SpinUntil(() => token.IsCancellationRequested, 1);
                    }                   
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex);
                }
                finally
                {
                    writer.Flush();
                    _queue.Writer.Complete();
                    streamWriter.Flush();
                    streamWriter.Close();
                    _logger.Debug("File stream and queue are closed");
                    _localCts.Dispose();
                }
            });
        }

        public async Task WriteAsync(T data)
        {
            await Task.Run(() =>
            {
                _queue!.Writer.TryWrite(data);
                _logger.Info("Data is queued to filestream");
            });
        }

        public void Write(object data)
        {
            WriteAsync(data).Wait();
        }

        public async Task WriteAsync(object data)
        {
            await WriteAsync((T)data);
        }

        public IRecorder CreateInstance()
        {
            return new CSVRecorder<T>();
        }
    }

    public class CSVRecorder : IRecorder
    {
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
        private Channel<object>? _queue;
        private CSVRecorderConfiguration? _config;
        private CancellationTokenSource _localCts = new CancellationTokenSource();
        private Task _task = Task.CompletedTask;

        public string Name => nameof(CSVRecorder);
        public void Load(IConfigurationSection section)
        {
            _config = section.Get<CSVRecorderConfiguration>()??CSVRecorderConfiguration.Default;;
            _queue = Channel.CreateBounded<object>(new BoundedChannelOptions(_config.QueueSize) { FullMode = _config.BoundedChannelFullMode });
            _logger.Info("CSVRecorder initiated");
        }
        public void LoadFromObject(object configuration)
        {
            _config = (configuration as CSVRecorderConfiguration) ?? CSVRecorderConfiguration.Default;
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
                            _logger.Debug($"Data is written to file, elements in queue:{_queue.Reader.Count}");
                        }
                        SpinWait.SpinUntil(() => token.IsCancellationRequested, 1);
                    }
                    writer.Flush();
                    _queue!.Writer.Complete();
                    streamWriter.Flush();
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
                _logger.Debug(JsonConvert.SerializeObject(data));
                _queue!.Writer.TryWrite(data);
                _logger.Info("Data is queued to filestream");
            });
        }
    }

    public class CSVRecorderConfiguration
    {
        public bool WaitForComplete { get; set; } = true;
        public int QueueSize { get; set; } = 2000;
        public int WaitForCompleteTimeout { get; set; } = 5000;
        public static CSVRecorderConfiguration Default = new CSVRecorderConfiguration();


        public BoundedChannelFullMode BoundedChannelFullMode { get; set; } = BoundedChannelFullMode.DropOldest;

    }
}