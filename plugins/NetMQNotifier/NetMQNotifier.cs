using Microsoft.Extensions.Configuration;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace MATSys.Plugins
{
    internal sealed class NetMQNotifier : INotifier
    {
        private PublisherSocket _pub = new PublisherSocket();
        private SubscriberSocket _sub = new SubscriberSocket();
        private bool isConnected = false;

        private NetMQNotifierConfiguration? _config;
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();

        public string Name => nameof(NetMQNotifier);

        public event INotifier.NewDataEvent? OnNewDataReadyEvent;

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

        public void StartService(CancellationToken token)
        {
            try
            {
                _pub.Bind($"{_config!.Protocal}://{_config.Address}");
                _sub.Connect($"{_config.Protocal}://{_config.Address}");
                _sub.Subscribe(_config.Topic);
                isConnected = true;
                _logger.Info("Starts service");
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            _config = section.Get<NetMQNotifierConfiguration>();
            _logger = _config.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger(); ;

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            _logger.Info("NetMQNotifier is initiated");
        }

        public void Load(object configuration)
        {
            _config = configuration as NetMQNotifierConfiguration;
            _logger = _config.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger(); ;

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            _logger.Info("NetMQNotifier is initiated");
        }

        public object GetData(int timeoutInMilliseconds)
        {
            return _sub.ReceiveMultipartStrings()[1];
        }

        ~NetMQNotifier()
        {
        }

        internal class NetMQNotifierConfiguration
        {
            public bool EnableLogging { get; set; } = false;
            public string Address { get; set; } = "";
            public string Protocal { get; set; } = "";
            public string Topic { get; set; } = "";
            public bool DisableEvent { get; set; } = true;
        }
    }
}