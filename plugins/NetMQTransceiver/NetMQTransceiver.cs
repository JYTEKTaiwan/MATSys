using Microsoft.Extensions.Configuration;
using NetMQ;

namespace MATSys.Plugins
{
    internal sealed class NetMQTransceiver : ITransceiver
    {
        private NetMQ.Sockets.RouterSocket _routerSocket = new NetMQ.Sockets.RouterSocket();
        private NetMQTransceiverConfiguration? _config;
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();

        public event ITransceiver.RequestFiredEvent? OnNewRequest;

        private CancellationTokenSource _localCts = new CancellationTokenSource();

        public string Name => nameof(NetMQTransceiver);

        public void StartService(CancellationToken token)
        {
            _localCts = new CancellationTokenSource();
            var _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);

            var address = $"{_config?.LocalIP}:{_config?.Port}";
            _logger.Trace("Prepare to run");
            _logger.Debug($"Binds to {address}");
            _routerSocket.Bind(address);

            _logger.Info("Start service");
            RoutingKey key = new RoutingKey();

            //Errors happened in the internal loop were clarified as Fatal error
            Task.Run(() =>
           {
               try
               {
                   while (!_linkedCts.IsCancellationRequested)
                   {
                       if (_routerSocket.TryReceiveRoutingKey(TimeSpan.FromSeconds(1), ref key))
                       {
                           //index 0: CommandBase object in json format
                           var content = _routerSocket.ReceiveMultipartStrings();
                           _logger.Debug($"New message received {content[0]}");

                           var response = OnNewRequest?.Invoke(this, content[0])!;
                           _logger.Trace("Message is executed");

                           _routerSocket.SendMoreFrame(key);
                           _routerSocket.SendFrame(response);
                           _logger.Debug($"Response sent back {response}");
                       }
                   }
               }
               catch (Exception ex)
               {
                   _logger.Fatal(ex);
               }
               finally
               {
                   _routerSocket.Unbind(address);
                   _logger.Debug($"Unbind to {address}");
                   _logger.Info("Stop service");
               }
           });
        }

        public void StopService()
        {
            _localCts.Cancel();
            _logger.Info("Stop service");
        }

        public void Load(IConfigurationSection section)
        {
            _config = section.Get<NetMQTransceiverConfiguration>();
            _logger = _config.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger(); ;

            _logger.Info("NetMQTransceiver is initiated");
        }

        public void Load(object configuration)
        {
            _config = configuration as NetMQTransceiverConfiguration;
            _logger =_config.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger(); ;

            _logger.Info("NetMQTransceiver is initiated");
        }

        internal class NetMQTransceiverConfiguration
        {
            public bool EnableLogging { get; set; } = false;
            public string AliasName { get; set; } = "";
            public string LocalIP { get; set; } = "";
            public int Port { get; set; } = -1;
        }
    }
}