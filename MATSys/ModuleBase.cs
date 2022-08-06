using MATSys.Commands;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using System.Reflection;

namespace MATSys
{
    public abstract class ModuleBase : IModule
    {
        public const string cmd_notFound = "NOTFOUND";
        public const string cmd_execError = "EXEC_ERROR";
        private readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        private readonly ILogger _logger;
        private readonly ITransceiver _transceiver;
        private readonly IRecorder _recorder;
        private readonly INotifier _notifier;
        public volatile bool _isRunning = false;
        private object _config;
        public bool IsRunning => _isRunning;

        public event IModule.NewDataReady? OnDataReady;

        ILogger IModule.Logger => _logger;
        IRecorder IModule.Recorder => _recorder;
        ITransceiver IModule.Transceiver => _transceiver;
        INotifier IModule.Notifier => _notifier;
        public string Name { get; }
        public IModule Base => this;
        public ModuleBase(object configuration, ITransceiver server, INotifier bus, IRecorder recorder, string configurationKey = "")
        {
            Name = string.IsNullOrEmpty(configurationKey) ? $"{GetType().Name}_{GetHashCode().ToString("X2")}" : configurationKey;

            _logger = LogManager.GetLogger(Name);

            _config = LoadAndSetup(configuration);
            _recorder = InjectRecorder(recorder);
            _notifier = InjectNotifier(bus);
            _transceiver = InjectTransceiver(server);
            _methods = ParseSupportedMethods();
            _logger.Info($"{Name} base class initialization is completed");
        }
        private object LoadAndSetup(object option)
        {
            object config = null;
            if (option != null)
            {
                config = option;
                if (option.GetType().IsAssignableFrom(typeof(IConfigurationSection)))
                {
                    Load((IConfigurationSection)option);
                }
                Load(option);
            }
            return config;
        }
        private IRecorder InjectRecorder(IRecorder recorder)
        {
            IRecorder _recorder = null;
            if (recorder == null)
            {
                _recorder = new EmptyRecorder();
                _logger.Info($"Null reference is detected, {_recorder.Name} is injected");
            }
            else
            {
                _recorder = recorder;
                _logger.Info($"{_recorder.Name} is injected");
            }
            return _recorder;
        }
        private ITransceiver InjectTransceiver(ITransceiver server)
        {
            ITransceiver transceiver = null;
            if (server == null)
            {
                transceiver = new EmptyTransceiver();
                _logger.Info($"Null reference is detected, {transceiver.Name} is injected");
            }
            else
            {
                transceiver = server;
                _logger.Info($"{transceiver.Name} is injected");
            }
            transceiver.OnCommandReady += OnCommandDataReady;
            return transceiver;
        }
        private INotifier InjectNotifier(INotifier bus)
        {
            INotifier notifier = null;
            if (bus == null)
            {
                notifier = new EmptyNotifier();
                _logger.Info($"Null reference is detected, {notifier.Name} is injected");
            }
            else
            {
                notifier = bus;
                _logger.Info($"{notifier.Name} is injected");
            }
            notifier.OnNewDataReadyEvent += NewDataReady;
            return notifier;

        }
        private void NewDataReady(string dataInJson)
        {
            OnDataReady?.Invoke(dataInJson);
        }
        private Dictionary<string, MethodInfo> ParseSupportedMethods()
        {
            var methodlist = GetType().GetMethods().Where(x =>
            {
                return x.GetCustomAttributes<MethodNameAttribute>(false).Count() > 0;
            }).ToArray();
            return methodlist.ToDictionary(x => x.GetCustomAttribute<MethodNameAttribute>()!.Name);
        }
        private string OnCommandDataReady(object sender, string commandObjectInJson)
        {
            try
            {
                var answer = "";
                _logger.Trace($"OnDataReady event fired");
                _logger.Debug($"New command object string is received: {commandObjectInJson}");
                var parsedName = commandObjectInJson.Split(new string[] { "MethodName\"" }, StringSplitOptions.RemoveEmptyEntries)[1].Split('\"')[1];
                if (_methods.ContainsKey(parsedName))
                {
                    var method = _methods[parsedName];
                    var att = method.GetCustomAttribute<MethodNameAttribute>();
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
        public virtual void StartService(CancellationToken token)
        {
            if (!_isRunning)
            {
                try
                {
                    _logger.Trace($"Starts the {_recorder.Name}");
                    _recorder.StartService(token);
                    _logger.Trace($"Starts the {_notifier.Name}");
                    _notifier.StartService(token);
                    _logger.Trace($"Starts the {_transceiver.Name}");
                    _transceiver.StartService(token);

                    _isRunning = true;
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
                if (_isRunning)
                {
                    _logger.Trace($"Stops the {_transceiver.Name}");
                    _transceiver.StopService();

                    _logger.Trace($"Stops the {_recorder.Name}");
                    _recorder.StopService();

                    _logger.Trace($"Stops the {_notifier.Name}");
                    _notifier.StopService();
                    _isRunning = false;
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
            if (_methods.ContainsKey(cmd.MethodName))
            {
                var method = _methods[cmd.MethodName];
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
        public abstract void Load(object configuration);
        public virtual IEnumerable<string> PrintCommands()
        {
            var cmds = GetType().GetMethods().Where(x => x.GetCustomAttributes<MethodNameAttribute>().Count() > 0);
            foreach (var item in cmds)
            {
                yield return item.GetCustomAttribute<MethodNameAttribute>()!.GetJsonString();
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