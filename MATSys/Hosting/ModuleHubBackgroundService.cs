﻿using MATSys.Commands;
using MATSys.Factories;
using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NLog;
using System;
using System.Xml.Linq;

namespace MATSys
{
    public sealed class ModuleHubBackgroundService : BackgroundService
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly IConfiguration _config;
        private readonly ITransceiver _transceiver;
        private readonly AutoTestScheduler _scheduler;
        private readonly ILogger _logger;
        private readonly bool _scriptMode = false;

        private CancellationTokenSource cts = new CancellationTokenSource();


        public delegate void ReadyToExecuteEvent(string module, string command);
        public event ReadyToExecuteEvent? OnReadyToExecute;
        public delegate void ExecuteCompleteEvent(TestItem item, string result);
        public event ExecuteCompleteEvent? OnExecuteComplete;

        public ModuleCollection Modules { get; } = new ModuleCollection();
        public ModuleHubBackgroundService(IServiceProvider services)
        {
            try
            {
                _logger = LogManager.GetCurrentClassLogger();
                _config = services.GetRequiredService<IConfiguration>().GetSection("MATSys");
                _scriptMode = _config.GetValue<bool>("ScriptMode");
                _moduleFactory = services.GetRequiredService<IModuleFactory>();
                foreach (var item in _config.GetSection("Modules").GetChildren())
                {
                    Modules.Add(item.GetValue<string>("Name"), _moduleFactory.CreateModule(item));
                }
                _transceiver = services.GetRequiredService<ITransceiverFactory>().CreateTransceiver(_config.GetSection("Transceiver"));
                _transceiver.OnNewRequest += _transceiver_OnNewRequest;
                _scheduler = services.GetRequiredService<AutoTestScheduler>();
            }
            catch (Exception ex)
            {
                throw new Exception($"ModuleHub Initialzation failed", ex);
            }
        }
        private string _transceiver_OnNewRequest(object sender, string commandObjectInJson)
        {
            //format is {Name}:{command in json formt}
            var name = commandObjectInJson.Split(':')[0];
            var cmd = commandObjectInJson.Split(':')[1];
            return ExecuteCommand(name, cmd);
        }

        public string ExecuteCommand(string name, ICommand cmd)
        {
            if (_scriptMode)
            {
                return "[Warn] Service is in script mode";
            }
            else
            {
                return Modules[name].Execute(cmd);
            }

        }
        public string ExecuteCommand(string name, string cmd)
        {
            if (_scriptMode)
            {
                return "[Warn] Service is in script mode";
            }
            else
            {
                return Modules[name].Execute(cmd);
            }
        }

        public void RunTest(int iteration)
        {
            if (_scriptMode)
            {
                cts = new CancellationTokenSource();
                Task.Run(() => AutoTesting(iteration, cts.Token));
            }

        }
        public void StopTest()
        {
            if (_scriptMode)
            {
                cts.Cancel();

            }
        }
        private void AutoTesting(int iteration, CancellationToken token)
        {
            _scheduler.RunSetup();
            _scheduler.RunTestItem();
            int cnt = 1;
            while (!cts.IsCancellationRequested)
            {
                if (cnt == iteration)
                {
                    cts.Cancel();
                }
                else if (!_scheduler.IsAvailable)
                {
                    _scheduler.RunTestItem();
                    Interlocked.Increment(ref cnt);
                }
                else
                {
                    SpinWait.SpinUntil(() => false, 5);
                }

            }
            _scheduler.RunTeardown();

        }
        private void Setup(CancellationToken stoppingToken)
        {
            foreach (var item in Modules.Values)
            {
                item.StartService(stoppingToken);
            }

        }
        private void Cleanup()
        {
            foreach (var item in Modules.Values)
            {
                item.StopService();
            }

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                Setup(stoppingToken);
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (SpinWait.SpinUntil(() => _scheduler.IsAvailable, 5))
                    {
                        var testItem = await _scheduler.Dequeue(stoppingToken);
                        OnReadyToExecute?.Invoke(testItem.ModuleName, testItem.Command);
                        var response = Modules[testItem.ModuleName].Execute(testItem.Command);
                        OnExecuteComplete?.Invoke(testItem, response);
                    }
                }
                Cleanup();
            });
        }
    }

    public sealed class ModuleCollection : Dictionary<string, IModule>
    {
        public ModuleCollection() : base()
        {
        }

    }

}