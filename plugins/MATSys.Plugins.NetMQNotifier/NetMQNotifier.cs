using Microsoft.Extensions.Configuration;
using NetMQ;
using NetMQ.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MATSys.Plugins
{
    /// <summary>
    /// Notifier implemented by NetMQ library
    /// </summary>
    public sealed class NetMQNotifier : INotifier
    {
        private PublisherSocket _pub = new PublisherSocket();
        private bool isConnected = false;
        private string latestString = "";

        private NetMQNotifierConfiguration? _config;
        private NLog.ILogger _logger = NLog.LogManager.CreateNullLogger();

        public string Alias { get; set; } = nameof(NetMQNotifier);

        public event INotifier.NotifyEvent? OnNotify;

        public void Publish(object data)
        {
            if (!isConnected)
            {
                Bind();
            }
            var json = JsonSerializer.Serialize(data);
            latestString = json;
            _logger.Trace("Ready to publish message");
            var msg = new NetMQMessage();
            msg.Append(_config!.Topic);
            msg.Append(json);
            _pub.TrySendMultipartMessage(msg);
            _logger.Debug($"Message content:{msg.ToString()}");
            _logger.Info("Message has sent");
            if (!_config.DisableEvent)
            {
                OnNotify?.Invoke(json);
            }
        }

        public void Bind()
        {
            try
            {
                _pub.Bind($"{_config!.Protocal}://{_config.Address}");
                isConnected = true;
                _logger.Info("Starts service");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Unbind()
        {
            if (isConnected)
            {
                _pub.Unbind(_pub.Options.LastEndpoint!);
                isConnected = false;
                _logger.Info("Stop service");
            }
        }

        public void Load(IConfigurationSection section)
        {
            _config = section.Get<NetMQNotifierConfiguration>();
            _logger = _config.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger(); ;

            _logger.Info("NetMQNotifier is initiated");
        }

        public void Load(object configuration)
        {
            _config = (NetMQNotifierConfiguration)configuration;
            _logger = _config.EnableLogging ? NLog.LogManager.GetCurrentClassLogger() : NLog.LogManager.CreateNullLogger();
            _logger.Info("NetMQNotifier is initiated");
        }

        public object GetData(int timeoutInMilliseconds)
        {
            return latestString;
        }
        public JsonObject Export()
        {
            return JsonObject.Parse(JsonSerializer.Serialize(_config)).AsObject();

        }
        public string Export(bool indented = true)
        {
            return Export().ToJsonString(new JsonSerializerOptions() { WriteIndented = indented = indented });
        }

        public void Dispose()
        {
            Unbind();
        }

        ~NetMQNotifier()
        {
        }

    }

    /// <summary>
    /// Configuration definition for NetMQNotifier
    /// </summary>
    public class NetMQNotifierConfiguration 
    {
        public string Type { get; set; } = "netmq";
        public bool EnableLogging { get; set; } = false;
        public string Address { get; set; } = "127.0.0.1:5000";
        public string Protocal { get; set; } = "tcp";
        public string Topic { get; set; } = "";
        public bool DisableEvent { get; set; } = true;

        public static NetMQNotifierConfiguration Default => new NetMQNotifierConfiguration();
    }
}