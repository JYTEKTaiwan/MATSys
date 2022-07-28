using MATSys.Commands;
using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;
using System.Reflection;

namespace MATSys
{
    public abstract class ModuleBase : IModule
    {
        private const string key_recorder = "Recorder";
        private const string key_notifier = "Notifier";
        private const string key_transceiver = "Transceiver";
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

        public ModuleBase(IServiceProvider services, string configurationKey)
        {
            try
            {
                var _config = services.GetRequiredService<IConfiguration>();
                //sec might be null (will be checked in the factory)
                var section = _config.GetSection(configurationKey);

                //set name of the Device base object
                Name = configurationKey;
                //Create internal logger using alias name
                _logger = LogManager.GetLogger(Name);

                Load(section);
                _recorder = services.GetRequiredService<IRecorderFactory>().CreateRecorder(section.GetSection(key_recorder));
                _logger.Trace($"{_recorder.Name} is injected");
                _notifier = services.GetRequiredService<INotifierFactory>().CreateNotifier(section.GetSection(key_notifier));
                _notifier.OnNewDataReadyEvent += NewDataReady;
                _logger.Trace($"{_notifier.Name} is injected");
                _transceiver = services.GetRequiredService<ITransceiverFactory>().CreateTransceiver(section.GetSection(key_transceiver));
                _logger.Trace($"{_transceiver.Name} is injected");
                _transceiver.OnCommandReady += OnCommandDataReady; ;
                _methods = ParseSupportedMethods();
                _logger.Info($"{Name} base class initialization is completed");
            }
            catch (Exception ex)
            {
                _logger?.Error(ex.Message);
                throw new Exception($"Initialization of DeviceBase failed", ex);
            }
        }
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
                _logger.Trace($"Null reference is detected, {this._recorder.Name} is injected");
            }
            else
            {
                _recorder = recorder;
                _logger.Trace($"{this._recorder.Name} is injected");
            }
            return _recorder;
        }
        private ITransceiver InjectTransceiver(ITransceiver server)
        {
            ITransceiver transceiver = null;
            if (server == null)
            {
                _logger.Trace($"Null reference is detected, {transceiver.Name} is injected");
                transceiver = new EmptyTransceiver();
            }
            else
            {
                _logger.Trace($"{server.Name} is injected");
                transceiver = server;
            }
            transceiver.OnCommandReady += OnCommandDataReady;
            return transceiver;
        }
        private INotifier InjectNotifier(INotifier bus)
        {
            INotifier notifier = null;
            if (bus == null)
            {
                _logger.Trace($"Null reference is detected, {_notifier.Name} is injected");
                notifier = new EmptyNotifier();

            }
            else
            {
                _logger.Trace($"{_notifier.Name} is injected");
                notifier = bus;
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
                    _logger.Trace("Starts the DataRecorder");
                    _recorder.StartService(token);

                    _logger.Trace("Starts the Publisher");
                    _notifier.StartService(token);

                    _logger.Trace("Starts the CommandServer");
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
                    _logger.Trace("Stops the CommandStream");
                    _transceiver.StopService();

                    _logger.Trace("Stops the DataRecorder");
                    _recorder.StopService();

                    _logger.Trace("Stops the Publisher");
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