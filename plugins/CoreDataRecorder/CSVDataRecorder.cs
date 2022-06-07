using CsvHelper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Channels;

namespace MATSys.Plugins
{
    public class CSVDataRecorder<T> : IDataRecorder<T> where T : class
    {
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
        private Channel<T>? _queue;
        private CSVDataRecorderConfiguration? _config;

        private CancellationTokenSource _localCts = new CancellationTokenSource();
        private Task _task = Task.CompletedTask;

        public string Name => nameof(CSVDataRecorder);

        public void Load(IConfigurationSection section)
        {
            _config = section.Get<CSVDataRecorderConfiguration>();
            _queue = Channel.CreateBounded<T>(new BoundedChannelOptions(_config.QueueSize) { FullMode = _config.BoundedChannelFullMode });
            _logger.Info("CSVDataRecorder initiated");
        }

        public void Write(T data)
        {
            WriteAsync(data).Wait();
        }

        public void Stop()
        {
            _localCts.Cancel();
            _logger.Info("Stop service");
        }

        private bool TaskStatusCheck(Task t)
        {
            return t.Status == TaskStatus.RanToCompletion || t.Status == TaskStatus.Canceled || t.Status == TaskStatus.Faulted;
        }

        public Task RunAsync(CancellationToken token)
        {
            _logger.Trace("Prepare to run");

            if (TaskStatusCheck(_task))
            {
                _task = Task.Run(() =>
                {
                    _localCts = new CancellationTokenSource();
                    var filename = DateTime.Now.ToString("HHmmssfff") + ".csv";
                    _queue = Channel.CreateBounded<T>(new BoundedChannelOptions(2000) { FullMode = BoundedChannelFullMode.DropOldest });
                    var _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
                    T? data;
                    StreamWriter streamWriter = new StreamWriter(filename);
                    CsvWriter writer = new CsvWriter(streamWriter, System.Globalization.CultureInfo.InvariantCulture);
                    writer.WriteHeader<T>();
                    writer.NextRecord();

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

                    writer.Flush();
                    _queue.Writer.Complete();
                    streamWriter.Flush();
                    streamWriter.Close();
                    _logger.Debug("File stream and queue are closed");

                    _localCts.Dispose();
                });
            }
            return _task;
        }

        public Task WriteAsync(T data)
        {
            return Task.Run(() =>
            {
                _queue!.Writer.TryWrite(data);
                _logger.Info("Data is queued to filestream");
            });
        }

        public void Write(object data)
        {
            WriteAsync(data).Wait();
        }

        public Task WriteAsync(object data)
        {
            return WriteAsync((T)data);
        }

        public IDataRecorder CreateInstance()
        {
            return new CSVDataRecorder<T>();
        }
    }

    public class CSVDataRecorder : IDataRecorder
    {
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
        private Channel<object>? _queue;
        private CSVDataRecorderConfiguration? _config;
        private CancellationTokenSource _localCts = new CancellationTokenSource();
        private Task _task = Task.CompletedTask;

        public string Name => nameof(CSVDataRecorder);

        public void Load(IConfigurationSection section)
        {
            _config = section.Get<CSVDataRecorderConfiguration>();
            _queue = Channel.CreateBounded<object>(new BoundedChannelOptions(_config.QueueSize) { FullMode = _config.BoundedChannelFullMode });
            _logger.Info("CSVDataRecorder initiated");
        }

        public void Write(object data)
        {
            WriteAsync(data).Wait();
        }

        public void Stop()
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

        public Task RunAsync(CancellationToken token)
        {
            return Task.Run(() =>
            {
                _logger.Trace("Prepare to run");
                if (TaskStatusCheck(_task))
                {
                    StartService(token);
                }
            });
        }

        private void StartService(CancellationToken token)
        {
            _task = Task.Run(() =>
            {
                _localCts = new CancellationTokenSource();
                var filename = DateTime.Now.ToString("HHmmssfff") + ".csv";
                var _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);
                object? data;
                StreamWriter streamWriter = new StreamWriter(filename);
                CsvWriter writer = new CsvWriter(streamWriter, System.Globalization.CultureInfo.InvariantCulture);

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

        public Task WriteAsync(object data)
        {
            return Task.Run(() =>
            {
                _logger.Debug(JsonConvert.SerializeObject(data));
                _queue!.Writer.TryWrite(data);
                _logger.Info("Data is queued to filestream");
            });
        }
    }

    public class CSVDataRecorderConfiguration
    {
        public bool WaitForComplete { get; set; } = true;
        public int QueueSize { get; set; } = 2000;
        public int WaitForCompleteTimeout { get; set; } = 5000;

        public BoundedChannelFullMode BoundedChannelFullMode { get; set; } = BoundedChannelFullMode.DropOldest;
    }
}