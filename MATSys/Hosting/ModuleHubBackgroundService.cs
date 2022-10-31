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
        /// Test items loaded from appsetting.json
        /// </summary>
        public TestItemCollection TestItems => _scheduler.TestItems;

        /// <summary>
        /// Setup items loaded from appsettings.json
        /// </summary>
        public TestItemCollection SetupItems => _scheduler.SetupItems;

        /// <summary>
        /// Teardown items loaded from appsettings.json
        /// </summary>
        public TestItemCollection TeardownItems => _scheduler.TeardownItems;


        /// <summary>
        /// Event when TestItem is ready to execute
        /// </summary>
        /// <param name="item">TestItem instance</param>
        public delegate void ReadyToExecuteEvent(TestItem item);

        /// <summary>
        /// Event when TestItem is ready to execute
        /// </summary>
        public event ReadyToExecuteEvent? BeforeTestItem;

        /// <summary>
        /// Event when TestItem is executed completely
        /// </summary>
        /// <param name="item">TestItem instsance</param>
        /// <param name="result">Executed result</param>
        public delegate void ExecuteCompleteEvent(TestItem item, string result);

        /// <summary>
        /// Event when TestItem is executed completely
        /// </summary>
        public event ExecuteCompleteEvent? AfterTestItem;

        /// <summary>
        /// Collection for all modules created the background servie
        /// </summary>
        public Dictionary<string, IModule> Modules { get; } = new Dictionary<string, IModule>();

        /// <summary>
        /// Constructor for the background service
        /// </summary>
        /// <param name="services">IServiceProvider instance</param>
        /// <exception> throw <see cref="Exception"/> for any exception</exception>
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
                    Modules.Add(item.GetValue<string>("Alias"), _moduleFactory.CreateModule(item));
                }
                foreach (var item in Modules)
                {
                    //Assign the LocalPeers properties (Modules can access each other instance locally)
                    item.Value.LocalPeers = Modules;
                }
                _transceiver = services.GetRequiredService<ITransceiverFactory>().CreateTransceiver(_config.GetSection("Transceiver"));
                _transceiver.OnNewRequest += _transceiver_OnNewRequest;
                _notifier = services.GetRequiredService<INotifierFactory>().CreateNotifier(_config.GetSection("Notifier"));
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
        /// <param name="iteration">iteration count (infinitely if value less than or equals to 0)</param>
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
                    var testItem = await _scheduler.Dequeue(stoppingToken);
                    if (!cts.IsCancellationRequested)
                    {
                        //when script is running
                        BeforeTestItem?.Invoke(testItem);
                        var response = Modules[testItem.ModuleName].Execute(testItem.Command);
                        _notifier.Publish(response);
                        AfterTestItem?.Invoke(testItem, response);
                    }
                    else if (testItem.Type == ScriptType.Test)
                    {
                        //StopTest() is called.
                        //TestItems are ignored
                    }
                    else
                    {
                        //StopTest() is called
                        //Setup and teardown items
                        BeforeTestItem?.Invoke(testItem);
                        var response = Modules[testItem.ModuleName].Execute(testItem.Command);
                        _notifier.Publish(response);
                        AfterTestItem?.Invoke(testItem, response);

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
            int cnt = 0;
            while (!token.IsCancellationRequested)
            {
                if (SpinWait.SpinUntil(() => !_scheduler.IsAvailable, 0))
                {
                    if (cnt == iteration && iteration != 0)
                    {
                        break;
                    }
                    else
                    {
                        _scheduler.AddTestItem();
                        Interlocked.Increment(ref cnt);
                    }
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