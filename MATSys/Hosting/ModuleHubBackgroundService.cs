using MATSys.Commands;
using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;

namespace MATSys.Hosting
{
    /// <summary>
    /// A service class that handles all modules in the background, including the ability to run in script mode.
    /// </summary>
    public sealed class ModuleHubBackgroundService : BackgroundService
    {
        private readonly IModuleFactory _moduleFactory;
        private readonly IConfiguration _config;
        private readonly ITransceiver _transceiver;
        private readonly INotifier _notifier;
        private readonly AutoTestScheduler _scheduler;
        private readonly ILogger _logger;
        private readonly bool _scriptMode = false;
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Collection of test item in the script section in appsetting.json
        /// </summary>
        public TestItem[] TestItems { get; }

        /// <summary>
        /// Event when TestItem is ready to execute
        /// </summary>
        /// <param name="item">TestItem instance</param>
        public delegate void ReadyToExecuteEvent(TestItem item);

        /// <summary>
        /// Event when TestItem is ready to execute
        /// </summary>
        public event ReadyToExecuteEvent? OnReadyToExecute;
        /// <summary>
        /// Event when TestItem is executed completely
        /// </summary>
        /// <param name="item">TestItem instsance</param>
        /// <param name="result">Executed result</param>
        public delegate void ExecuteCompleteEvent(TestItem item, string result);
        /// <summary>
        /// Event when TestItem is executed completely
        /// </summary>
        public event ExecuteCompleteEvent? OnExecuteComplete;

        /// <summary>
        /// Collection for all modules created the background servie
        /// </summary>
        public Dictionary<string, IModule> Modules { get; } = new Dictionary<string, IModule>();
        /// <summary>
        /// Constructor for the background service
        /// </summary>
        /// <param name="services">IServiceProvider instance</param>
        /// <exception cref="Exception">Every exception is included</exception>
        public ModuleHubBackgroundService(IServiceProvider services)
        {
            try
            {
                _logger = LogManager.GetCurrentClassLogger();
                _config = services.GetRequiredService<IConfiguration>().GetSection("MATSys");
                _scriptMode = _config.GetValue<bool>("ScriptMode");
                TestItems = TestScript.GetTestItems(_config.GetSection("Scripts").Get<TestScript>().Test).ToArray();
                _moduleFactory = services.GetRequiredService<IModuleFactory>();
                foreach (var item in _config.GetSection("Modules").GetChildren())
                {
                    Modules.Add(item.GetValue<string>("Alias"), _moduleFactory.CreateModule(item));
                }
                foreach (var item in Modules)
                {
                    //Assign the LocalPeers properties (Modules can access each other instance locally)
                    item.Value.LocalPeers = Modules;
                }
                _transceiver = services.GetRequiredService<ITransceiverFactory>().CreateTransceiver(_config.GetSection("Transceiver"));
                _transceiver.OnNewRequest += _transceiver_OnNewRequest;
                _notifier = services.GetService<INotifierFactory>().CreateNotifier(_config.GetSection("Notifier"));
                _scheduler = services.GetRequiredService<AutoTestScheduler>();
            }
            catch (Exception ex)
            {
                throw new Exception($"ModuleHub Initialzation failed", ex);
            }
        }

        /// <summary>
        /// Executes sommand 
        /// </summary>
        /// <param name="name">Module name</param>
        /// <param name="cmd">ICommand instance</param>
        /// <returns>Response from module</returns>
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

        /// <summary>
        /// Executes command
        /// </summary>
        /// <param name="name">Module name</param>
        /// <param name="cmd">Command string</param>
        /// <returns>Response from module</returns>
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

        /// <summary>
        /// Run the test in script mode (ScripMode property must be set to true in advance)
        /// </summary>
        /// <param name="iteration">iteration count (infinitely if value<=0)</param>
        public void RunTest(int iteration)
        {
            if (_scriptMode)
            {
                cts = new CancellationTokenSource();
                Task.Run(() => AutoTesting(iteration, cts.Token));
            }

        }
        /// <summary>
        /// Stop the test in script mode
        /// </summary>
        public void StopTest()
        {
            if (_scriptMode)
            {
                cts.Cancel();

            }
        }

        /// <summary>
        /// This method will be called immediately once the host is started
        /// </summary>
        /// <param name="stoppingToken">Stop token</param>
        /// <returns>Task of the execution logic</returns>
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
                        OnReadyToExecute?.Invoke(testItem);
                        var response = Modules[testItem.ModuleName].Execute(testItem.Command);
                        _notifier.Publish(response);
                         OnExecuteComplete?.Invoke(testItem, response);
                    }
                }
                Cleanup();
            });
        }

        private string _transceiver_OnNewRequest(object sender, string commandObjectInJson)
        {
            //format is {Name}:{command in json formt}
            var name = commandObjectInJson.Split(':')[0];
            var cmd = commandObjectInJson.Split(':')[1];
            return ExecuteCommand(name, cmd);
        }

        private void AutoTesting(int iteration, CancellationToken token)
        {
            _scheduler.AddSetupItem();
            _scheduler.AddTestItem();
            int cnt = 1;
            while (!token.IsCancellationRequested)
            {
                if (cnt == iteration)
                {
                    cts.Cancel();
                }
                else if (!_scheduler.IsAvailable)
                {
                    _scheduler.AddTestItem();
                    Interlocked.Increment(ref cnt);
                }
                else
                {
                    SpinWait.SpinUntil(() => false, 5);
                }

            }
            _scheduler.AddTearDownItem();
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
    }

}