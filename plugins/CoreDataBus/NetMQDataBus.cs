using Microsoft.Extensions.Configuration;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace MATSys.Plugins
{
    internal sealed class NetMQDataBus : IDataBus
    {
        private PublisherSocket _pub = new PublisherSocket();
        private SubscriberSocket _sub = new SubscriberSocket();
        private bool isConnected = false;

        private NetMQConfiguration? _config;
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name => nameof(NetMQDataBus);

        public event IDataBus.NewDataEvent? OnNewDataReadyEvent;

        private void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            _logger.Trace("ProcessExit event fired");
            if (isConnected)
            {
                _pub.Unbind(_pub.Options.LastEndpoint!);
                isConnected = false;
            }
            _pub.Dispose();
        }

        public void Publish(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            _logger.Trace("Ready to publish message");
            var msg = new NetMQMessage();
            msg.Append(_config!.Topic);
            msg.Append(json);
            _pub.TrySendMultipartMessage(msg);
            _logger.Debug($"Message content:{msg.ToString()}");
            _logger.Info("Message has sent");
            if (!_config.DisableEvent)
            {
                OnNewDataReadyEvent?.Invoke(json);
            }
        }

        public Task StartServiceAsync(CancellationToken token)
        {
            return Task.Run(() =>
            {
                _pub.Bind($"{_config!.Protocal}://{_config.Address}");
                _sub.Connect($"{_config.Protocal}://{_config.Address}");
                _sub.Subscribe(_config.Topic);
                isConnected = true;
                _logger.Info("Starts service");
            });
        }

        public void StopService()
        {
            if (isConnected)
            {
                _sub.Disconnect(_sub.Options.LastEndpoint!);
                _pub.Unbind(_pub.Options.LastEndpoint!);
                isConnected = false;
                _logger.Info("Stop service");
            }
        }

        public void Load(IConfigurationSection section)
        {
            _config = section.Get<NetMQConfiguration>();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            _logger.Info("NetMQPublisher is initiated");
        }

        public object GetData(int timeoutInMilliseconds)
        {
            return _sub.ReceiveMultipartStrings()[1];
        }

        ~NetMQDataBus()
        {
        }

        internal class NetMQConfiguration
        {
            public string Address { get; set; } = "";
            public string Protocal { get; set; } = "";
            public string Topic { get; set; } = "";
            public bool DisableEvent { get; set; } = true;
        }
    }
}