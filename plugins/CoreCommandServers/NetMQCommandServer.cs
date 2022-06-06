﻿using Microsoft.Extensions.Configuration;
using NetMQ;

namespace MATSys.Plugins
{
    internal sealed class NetMQCommandServer : ICommandServer
    {
        private NetMQ.Sockets.RouterSocket _routerSocket;
        private NetMQCommandStreamConfiguration _config;
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public event ICommandServer.CommandReadyEvent OnCommandReady;

        private CancellationTokenSource _localCts = new CancellationTokenSource();

        public string Name => nameof(NetMQCommandServer);

        public Task RunAsync(CancellationToken token)
        {
            return Task.Run(() =>
            {
                StartService(token);
            });
        }

        private void StartService(CancellationToken token)
        {
            _localCts = new CancellationTokenSource();
            var _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_localCts.Token, token);

            var address = $"{_config.LocalIP}:{_config.Port}";
            _logger.Trace("Prepare to run");
            _logger.Debug($"Binds to {address}");
            _routerSocket.Bind(address);

            Task.Run(() =>
            {
                _logger.Info("Start service");
                RoutingKey key = new RoutingKey();
                while (!_linkedCts.IsCancellationRequested)
                {
                    if (_routerSocket.TryReceiveRoutingKey(TimeSpan.FromSeconds(1), ref key))
                    {
                        //index 0: CommandBase object in json format
                        var content = _routerSocket.ReceiveMultipartStrings();
                        _logger.Debug($"New message received {content[0]}");

                        var response = OnCommandReady?.Invoke(this, content[0]);
                        _logger.Trace("Message is executed");

                        _routerSocket.SendMoreFrame(key);
                        _routerSocket.SendFrame(response);
                        _logger.Debug($"Response sent back {response}");
                    }
                }
                _routerSocket.Unbind(address);
                _logger.Debug($"Unbind to {address}");
                _logger.Info("Stop service");
            });
        }

        public void Stop()
        {
            _localCts.Cancel();
            _logger.Info("Stop service");
        }

        public void Load(IConfigurationSection section)
        {
            _config = section.Get<NetMQCommandStreamConfiguration>();
            _routerSocket = new NetMQ.Sockets.RouterSocket();
            _logger.Info("NetMQCommandServer is initiated");
        }

        internal class NetMQCommandStreamConfiguration
        {
            public string AliasName { get; set; } = "";
            public string LocalIP { get; set; } = "";
            public int Port { get; set; } = -1;
        }
    }
}