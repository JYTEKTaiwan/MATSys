using MATSys.Commands;
using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;
using System.Reflection;

namespace MATSys.Plugins
{
    public abstract class ModuleBase : IModule
    {
        private const string key_dataRecorder = "Recorder";
        private const string key_publisher = "Notifier";
        private const string key_server = "Transceiver";
        public const string cmd_notFound = "NOTFOUND";
        public const string cmd_execError = "EXEC_ERROR";
        private readonly Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        private readonly ILogger _logger;
        private readonly ITransceiver transceiverr;
        private readonly IRecorder _recorder;
        private readonly INotifier _notifier;
        public volatile bool isRunning = false;
        private object _config;
        public bool IsRunning => isRunning;

        public event IModule.NewDataReady? OnDataReady;

        ILogger IModule.Logger => _logger;
        IRecorder IModule.Recorder => _recorder;
        ITransceiver IModule.Transceiver => transceiverr;
        INotifier IModule.Notifier => _notifier;
        public string Name { get; }
        public IModule Instance => this;

        public ModuleBase(IServiceProvider services, string configurationKey)
        {
            try
            {
                var config = services.GetRequiredService<IConfiguration>();
                //sec might be null (will be checked in the factory)
                var section = config.GetSection(configurationKey);

                //set name of the Device base object
                Name = configurationKey;
                LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
                //Create internal logger using alias name
                _logger = NLog.LogManager.GetLogger(Name);

                Load(section);
                _recorder = services.GetRequiredService<IRecorderFactory>().CreateRecorder(section.GetSection(key_dataRecorder));
                _logger.Trace($"{_recorder.Name} is injected");
                _notifier = services.GetRequiredService<INotifierFactory>().CreateNotifier(section.GetSection(key_publisher));
                _notifier.OnNewDataReadyEvent += NewDataReady;
                _logger.Trace($"{_notifier.Name} is injected");
                transceiverr = services.GetRequiredService<ITransceiverFactory>().CreateTransceiver(section.GetSection(key_server));
                _logger.Trace($"{transceiverr.Name} is injected");
                transceiverr.OnCommandReady += OnCommandDataReady; ;
                methods = ParseSupportedMethods();
                _logger.Info($"{Name} base class initialization is completed");
            }
            catch (Exception ex)
            {
                _logger?.Error(ex.Message);
                throw new Exception($"Initialization of DeviceBase failed", ex);
            }
        }

        public ModuleBase(object option, ITransceiver server, INotifier bus, IRecorder recorder)
        {
            Name = $"{this.GetType().Name}_{this.GetHashCode().ToString("X2")}";
            _logger = NLog.LogManager.GetLogger(Name);

            if (option != null)
            {
                _config = option;
                LoadFromObject(option);
            }
            if (recorder == null)
            {
                _recorder = new EmptyRecorder();
                _logger.Trace($"Null reference is detected, {_recorder.Name} is injected");
            }
            else
            {
                _recorder = recorder;
                _logger.Trace($"{_recorder.Name} is injected");
            }

            if (bus == null)
            {
                _notifier = new EmptyNotifier();
                _logger.Trace($"Null reference is detected, {_notifier.Name} is injected");
            }
            else
            {
                _notifier = bus;
                _logger.Trace($"{_notifier.Name} is injected");
            }

            _notifier.OnNewDataReadyEvent += NewDataReady;

            if (server == null)
            {
                transceiverr = new EmptyTransceiver();
                _logger.Trace($"Null reference is detected, {transceiverr.Name} is injected");
            }
            else
            {
                transceiverr = server;
                _logger.Trace($"{transceiverr.Name} is injected");
            }

            transceiverr.OnCommandReady += OnCommandDataReady; ;
            methods = ParseSupportedMethods();
            _logger.Info($"{Name} base class initialization is completed");
        }

        private void NewDataReady(string dataInJson)
        {
            OnDataReady?.Invoke(dataInJson);
        }

        private Dictionary<string, MethodInfo> ParseSupportedMethods()
        {
            var methodlist = GetType().GetMethods().Where(x =>
            {
                return x.GetCustomAttributes<PrototypeAttribute>(false).Count() > 0;
            }).ToArray();
            return methodlist.ToDictionary(x => x.GetCustomAttribute<PrototypeAttribute>()!.Name);
        }

        private string OnCommandDataReady(object sender, string commandObjectInJson)
        {
            try
            {
                var answer = "";
                _logger.Trace($"OnDataReady event fired");
                _logger.Debug($"New command object string is received: {commandObjectInJson}");
                var parsedName = commandObjectInJson.Split(new string[] { "MethodName\"" }, StringSplitOptions.RemoveEmptyEntries)[1].Split('\"')[1];
                if (methods.ContainsKey(parsedName))
                {
                    var method = methods[parsedName];
                    var att = method.GetCustomAttribute<PrototypeAttribute>();
                    var cmd = JsonConvert.DeserializeObject(commandObjectInJson, att!.CommandType) as ICommand;
                    _logger.Debug($"Converted to command object successfully: {cmd!.MethodName}");

                    answer = Execute(cmd);

                    _logger.Debug($"Command object execution completed with return value {answer}");
                    _logger.Info($"Command [{cmd.MethodName}] is executed successfully");
                    return answer;
                }
                else
                {
                    return $"{cmd_notFound}:[{parsedName}]";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new Exception($"Execution of command ready event failed", ex);
            }
        }

        public void StartService(CancellationToken token)
        {
            if (!isRunning)
            {
                try
                {
                    _logger.Trace("Starts the DataRecorder");
                    _recorder.StartService(token);

                    _logger.Trace("Starts the Publisher");
                    _notifier.StartService(token);

                    _logger.Trace("Starts the CommandServer");
                    transceiverr.StartService(token);

                    isRunning = true;
                    _logger.Info("Starts service");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw new Exception($"RunAsync failed", ex);
                }
            }
        }

        public virtual void StopService()
        {
            try
            {
                if (isRunning)
                {
                    _logger.Trace("Stops the CommandStream");
                    transceiverr.StopService();

                    _logger.Trace("Stops the DataRecorder");
                    _recorder.StopService();

                    _logger.Trace("Stops the Publisher");
                    _notifier.StopService();
                    isRunning = false;
                    _logger.Info("Stops service");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new Exception("Stop failed", ex);
            }
        }

        public string Execute(ICommand cmd)
        {
            if (methods.ContainsKey(cmd.MethodName))
            {
                var method = methods[cmd.MethodName];
                try
                {
                    var result = method.Invoke(this, cmd.GetParameters())!;
                    return cmd.ConvertResultToString(result)!;
                }
                catch (Exception ex)
                {
                    bool check = ex is TargetException | ex is ArgumentException |
                    ex is TargetParameterCountException | ex is MethodAccessException | ex is InvalidOperationException |
                    ex is NotSupportedException;
                    if (check)
                    {
                        //exceptio from Invoke method
                        _logger.Warn(ex);
                        return $"{cmd_execError}: [{ex}]";
                    }
                    else
                    {
                        //custom class error, use inner error
                        _logger.Warn(ex.InnerException);
                        return $"{cmd_execError}: [{ex.InnerException}]";
                    }
                }
            }
            else
            {
                var res = $"{cmd_notFound}: [{cmd.MethodName}]";
                _logger.Warn(res);
                return res;
            }
        }

        public abstract void Load(IConfigurationSection section);

        public abstract void LoadFromObject(object configuration);

        public virtual IEnumerable<string> PrintCommands()
        {
            var cmds = GetType().GetMethods().Where(x => x.GetCustomAttributes<PrototypeAttribute>().Count() > 0);
            foreach (var item in cmds)
            {
                yield return item.GetCustomAttribute<PrototypeAttribute>()!.GetJsonString();
            }
        }

        public string Execute(string cmdInJson)
        {
            try
            {
                var result = OnCommandDataReady(this, cmdInJson);
                return result!;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new Exception($"Execute command failed", ex);
            }
        }
    }
}