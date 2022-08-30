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
        /// <summary>
        /// ctor of ModuleBase class. 
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="transceiver">transceiver instance</param>
        /// <param name="notifier">notifier instance</param>
        /// <param name="recorder">recorder instance</param>
        /// <param name="aliasName">alias name</param>
        public ModuleBase(object configuration, ITransceiver transceiver, INotifier notifier, IRecorder recorder, string aliasName = "")
        {
            Name = string.IsNullOrEmpty(aliasName) ? $"{GetType().Name}_{GetHashCode().ToString("X2")}" : aliasName;

            _logger = LogManager.GetLogger(Name);

            _config = LoadAndSetup(configuration);
            _recorder = InjectRecorder(recorder);
            _notifier = InjectNotifier(notifier);
            _transceiver = InjectTransceiver(transceiver);
            _methods = ParseSupportedMethods();
            _logger.Info($"{Name} base class initialization is completed");
        }
        /// <summary>
        /// load the configuration object
        /// </summary>
        /// <param name="option">parameter object</param>
        /// <returns>configuration instance (null if <paramref name="option"/> is null</returns>
        private object LoadAndSetup(object option)
        {
            if (option != null)
            {
                if (typeof(IConfigurationSection).IsAssignableFrom(option.GetType()))
                {
                    Load((IConfigurationSection)option);
                }
                else
                    Load(option);
            }
            return null;
        }
        /// <summary>
        /// Inject the IRecorder instance
        /// </summary>
        /// <param name="recorder">recorder instance</param>
        /// <returns>IRecorder instance (return EmptyRecorder if <paramref name="recorder"/> is null</returns>
        private IRecorder InjectRecorder(IRecorder recorder)
        {
            if (recorder == null)
            {
                var _recorder = new EmptyRecorder();
                _logger.Info($"Null reference is detected, {_recorder.Name} is injected");
                return _recorder;
            }
            else
            {
                _logger.Info($"{recorder.Name} is injected");
                return recorder;
            }

        }
        /// <summary>
        /// Inject the ITransceiver instance
        /// </summary>
        /// <param name="transceiver">ITransceiver instance</param>
        /// <returns>ITransceiver instance (return EmptyTransceiver if <paramref name="transceiver"/> is null</returns>
        private ITransceiver InjectTransceiver(ITransceiver transceiver)
        {
            if (transceiver == null)
            {
                var _transceiver = new EmptyTransceiver();
                _transceiver.OnNewRequest += OnRequestReceived;
                _logger.Info($"Null reference is detected, {_transceiver.Name} is injected");
                return _transceiver;
            }
            else
            {
                transceiver.OnNewRequest += OnRequestReceived;
                _logger.Info($"{transceiver.Name} is injected");
                return transceiver;
            }


        }
        /// <summary>
        /// Inject the INotifier instance
        /// </summary>
        /// <param name="notifier">INotifier instance</param>
        /// <returns>INotifier instance (return EmptyNotifier if <paramref name="notifier"/> is null</returns>
        private INotifier InjectNotifier(INotifier notifier)
        {
            if (notifier == null)
            {
                var _notifier = new EmptyNotifier();
                _notifier.OnNotify += OnNewDateGenerated;
                _logger.Info($"Null reference is detected, {_notifier.Name} is injected");
                return _notifier;
            }
            else
            {
                notifier.OnNotify += OnNewDateGenerated;
                _logger.Info($"{notifier.Name} is injected");
                return notifier;
            }

        }
        /// <summary>
        /// Event when new data is generated from ModuleBase internally
        /// </summary>
        /// <param name="dataInJson"></param>
        private void OnNewDateGenerated(string dataInJson)
        {
            OnDataReady?.Invoke(dataInJson);
        }
        /// <summary>
        /// Get the all methods with MethodNameAttribute assigned
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, MethodInfo> ParseSupportedMethods()
        {
            var methodlist = GetType().GetMethods().Where(x =>
            {
                return x.GetCustomAttributes<MethodNameAttribute>(false).Count() > 0;
            }).ToArray();
            return methodlist.ToDictionary(x => x.GetCustomAttribute<MethodNameAttribute>()!.Name);
        }
        /// <summary>
        /// Event when new request is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="commandObjectInJson"></param>
        /// <returns></returns>
        private string OnRequestReceived(object sender, string commandObjectInJson)
        {
            try
            {
                var answer = "";
                _logger.Trace($"OnDataReady event fired: {commandObjectInJson}");                
                var parsedName = commandObjectInJson.Split('=')[0];
                if (_methods.ContainsKey(parsedName))
                {
                    var method = _methods[parsedName];
                    var att = method.GetCustomAttribute<MethodNameAttribute>();
                    var cmd = att.Deserialize(commandObjectInJson);
                    _logger.Debug($"Converted to command object successfully: {cmd!.MethodName}");
                    answer = Execute(cmd);
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
                throw new Exception($"Execution of command failed", ex);
            }
        }
        /// <summary>
        /// Start the service
        /// </summary>
        /// <param name="token"></param>
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
        /// <summary>
        /// Stop the service
        /// </summary>
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
        /// <summary>
        /// Execute the incoming command object
        /// </summary>
        /// <param name="cmd">ICommand instance</param>
        /// <returns>reponse after execuing the commnad</returns>
        public string Execute(ICommand cmd)
        {
            if (_methods.ContainsKey(cmd.MethodName))
            {
                var method = _methods[cmd.MethodName];
                try
                {
                    _logger.Trace($"Command is ready to executed {cmd.SimplifiedString()}");
                    var result = method.Invoke(this, cmd.GetParameters())!;
                    var response = cmd.ConvertResultToString(result)!;
                    _logger.Debug($"Command [{cmd.MethodName}] is executed with return value: {response}");
                    _logger.Info($"Command [{cmd.MethodName}] is executed successfully");
                    return response;
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
        /// <summary>
        /// Execute the incoming command object in string format
        /// </summary>
        /// <param name="cmdInJson"></param>
        /// <returns></returns>
        public string Execute(string cmdInJson)
        {
            try
            {
                var result = OnRequestReceived(this, cmdInJson);
                return result!;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new Exception($"Execute command failed", ex);
            }
        }

        public async Task<string> ExecuteAsync(ICommand cmd)
        {
           return await Task.Run(()=> 
            {
                Monitor.Enter(this);
                var response=Execute(cmd);
                Monitor.Exit(this);
                return response;
            });
        }

        public async Task<string> ExecuteAsync(string cmdInJson)
        {
            return await Task.Run(() =>
            {
                Monitor.Enter(this);
                var response = Execute(cmdInJson);
                Monitor.Exit(this);
                return response;
            });
        }

        public abstract void Load(IConfigurationSection section);
        public abstract void Load(object configuration);

        /// <summary>
        /// List all methods 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> PrintCommands()
        {
            var cmds = GetType().GetMethods().Where(x => x.GetCustomAttributes<MethodNameAttribute>().Count() > 0);
            foreach (var item in cmds)
            {
                yield return item.GetCustomAttribute<MethodNameAttribute>()!.GetJsonString();
            }
        }
    }
}