using Microsoft.Extensions.Configuration;
using NetMQ;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MATSys.Plugins
{
    /// <summary>
    /// Transceiver implemented by NetMQ library
    /// </summary>
    public sealed class NetMQTransceiver : ITransceiver
    {
        private NetMQ.Sockets.RouterSocket _routerSocket = new NetMQ.Sockets.RouterSocket();
        private NetMQTransceiverConfiguration? _config;
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();
        private bool _isRunning = false;
        public event ITransceiver.RequestFiredEvent? OnNewRequest;

        private CancellationTokenSource _localCts = new CancellationTokenSource();

        public string Alias { get; set; } = nameof(NetMQTransceiver);

        public Task StartListening()
        {
            _localCts = new CancellationTokenSource();

            var address = $"{_config?.LocalIP}:{_config?.Port}";
            _logger.Trace("Prepare to run");
            _logger.Debug($"Binds to {address}");
            _routerSocket.Bind(address);

            _logger.Info("Start service");
            RoutingKey key = new RoutingKey();

            //Errors happened in the internal loop were clarified as Fatal error
            return Task.Run(() =>
            {
                try
                {
                    _isRunning = true;
                    while (!_localCts.IsCancellationRequested)
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

        public void StopListening()
        {
            _localCts.Cancel();
            _logger.Info("Stop service");
            _isRunning=false;
        }

        public void Load(IConfigurationSection section)
        {
            _config = section.Get<NetMQTransceiverConfiguration>();
            if (!_isRunning)
            {
                StartListening();
            }
            _logger = _config!.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger(); ;

            _logger.Info("NetMQTransceiver is initiated");
        }

        public void Load(object configuration)
        {
            _config = (NetMQTransceiverConfiguration)configuration;
            if (!_isRunning)
            {
                StartListening();
            }
            _logger = _config.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger(); ;

            _logger.Info("NetMQTransceiver is initiated");
        }
        public JsonObject Export()
        {
            return JsonObject.Parse(JsonSerializer.Serialize(_config))!.AsObject();

        }
        public string Export(bool indented = true)
        {
            return Export().ToJsonString(new JsonSerializerOptions() { WriteIndented = indented });
        }

        public void Dispose()
        {
            StopListening();
        }
    }

    /// <summary>
    /// Configuration definition for NetMQTransceiver
    /// </summary>
    public class NetMQTransceiverConfiguration
    {
        public string Type { get; set; } = "netmq";
        public bool EnableLogging { get; set; } = false;
        public string LocalIP { get; set; } = "";
        public int Port { get; set; } = -1;
    }
}