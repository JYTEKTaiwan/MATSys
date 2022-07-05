﻿using Microsoft.Extensions.Configuration;

namespace MATSys.Plugins
{
    public sealed class EmptyTransceiver : ITransceiver
    {
        private readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
        public string Name => nameof(EmptyTransceiver);

        public event ITransceiver.CommandReadyEvent? OnCommandReady;

        public void Load(IConfigurationSection section)
        {
            _logger.Info($"{nameof(EmptyTransceiver)} is initiated");
        }

        public void LoadFromObject(object configuration)
        {
            _logger.Info($"{nameof(EmptyTransceiver)} is initiated");
        }

        public void StartService(CancellationToken token)
        {
        }

        public void StopService()
        {
        }
    }
}