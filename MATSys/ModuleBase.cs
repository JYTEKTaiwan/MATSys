using MATSys.Commands;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Data;
using System.Reflection;
using System.Text.Json.Nodes;

namespace MATSys
{
    /// <summary>
    /// Abstract class equipped with ITransceiver, IRecorder, INotifier plugins
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        #region Private Fields
        /// <summary>
        /// Internal features injected
        /// </summary>
        private ILogger? _logger;
        private ITransceiver _transceiver = new EmptyTransceiver();
        private IRecorder _recorder = new EmptyRecorder();
        private INotifier _notifier = new EmptyNotifier();

        /// <summary>
        /// private field to use
        /// </summary>
        private volatile bool _isRunning = false;
        private Dictionary<string, MATSysCommandAttribute> cmds;
        private object? _config;
        #endregion

        #region Public Properties
        /// <summary>
        /// Running status of ModuleBase instance
        /// </summary>
        public bool IsRunning => _isRunning;
        /// <summary>
        /// Name of the ModuleBase instance
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// Instance of current ModuleBase instance
        /// </summary>
        public IModule Base => this;

        /// <summary>
        /// Collection of IModules created by ModuleHubBackgroundService
        /// </summary>
        public Dictionary<string, IModule> Peers { get; set; } = new Dictionary<string, IModule>();

        /// <summary>
        /// ILogger instance
        /// </summary>
        ILogger? IModule.Logger => _logger;

        /// <summary>
        /// IRecorder instance
        /// </summary>
        IRecorder IModule.Recorder => _recorder;

        /// <summary>
        /// ITransceiver instance
        /// </summary>
        ITransceiver IModule.Transceiver => _transceiver;

        /// <summary>
        /// INotifier instance
        /// </summary>
        INotifier IModule.Notifier => _notifier;
        /// <summary>
        /// Event when new data is generated inside ModuleBase instance
        /// </summary>
        public event IModule.NewDataReady? OnDataReady;

        #endregion


        #region Public Methods
        /// <summary>
        /// Load condfiguration from IConfigurationSection instance
        /// </summary>
        /// <param name="section"></param>
        public virtual void Load(IConfigurationSection section)
        {
            //do nothing(let user to assign the logic)
        }

        /// <summary>
        /// Load condfiguration from custom instance
        /// </summary>
        /// <param name="configuration"></param>
        public virtual void Load(object configuration)
        {
            //do nothing(let user to assign the logic)
        }


        /// <summary>
        /// Start the service
        /// </summary>
        /// <param name="token"></param>
        public virtual void StartPluginService(CancellationToken token)
        {
            if (!_isRunning)
            {
                try
                {
                    _logger?.Trace($"Starts the {_recorder.Alias}");
                    _recorder.StartPluginService(token);
                    _logger?.Trace($"Starts the {_notifier.Alias}");
                    _notifier.StartPluginService(token);
                    _logger?.Trace($"Starts the {_transceiver.Alias}");
                    _transceiver.StartPluginService(token);
                    _isRunning = true;
                    _logger?.Info("Starts service");
                }
                catch (Exception ex)
                {
                    _logger?.Error(ex);
                    throw new Exception($"RunAsync failed", ex);
                }
            }
        }
        /// <summary>
        /// Stop the service
        /// </summary>
        public virtual void StopPluginService()
        {
            try
            {
                if (_isRunning)
                {
                    _logger?.Trace($"Stops the {_transceiver.Alias}");
                    _transceiver.StopPluginService();

                    _logger?.Trace($"Stops the {_recorder.Alias}");
                    _recorder.StopPluginService();

                    _logger?.Trace($"Stops the {_notifier.Alias}");
                    _notifier.StopPluginService();
                    _isRunning = false;
                    _logger?.Info("Stops service");
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex);
                throw new Exception("Stop failed", ex);
            }
        }

        /// <summary>
        /// Execute the incoming command object
        /// </summary>
        /// <param name="cmd">ICommand instance</param>
        /// <returns>reponse after executing the commnad</returns>
        public string Execute(ICommand cmd)
        {
            try
            {
                _logger?.Trace($"Command is ready to executed {cmd.MethodName}");
                var invoker = cmds[cmd.MethodName].Invoker;
                if (invoker == null)
                {
                    throw new NullReferenceException("invoker is null");
                }
                var result = invoker.Invoke(cmd.GetParameters());
                var response = cmd.ConvertResultToString(result)!;
                _logger?.Debug($"Command [{cmd.MethodName}] is executed with return value: {response}");
                _logger?.Info($"Command [{cmd.MethodName}] is executed successfully");
                return response;
            }
            catch (KeyNotFoundException ex)
            {
                _logger?.Warn(ex);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_notFound, ex, cmd);

            }
            catch (Exception ex)
            {
                _logger?.Warn(ex);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_execError, ex, cmd);
            }
        }

        /// <summary>
        /// Execute the incoming command object in string format
        /// </summary>
        /// <param name="cmdInJson">command in string format</param>
        /// <returns>reponse after executing the commnad</returns>
        public string Execute(string cmdInJson)
        {
            var result = OnRequestReceived(this, cmdInJson);
            return result!;
        }

        /// <summary>
        /// List all methods in simplied string format 
        /// </summary>
        /// <returns>collection of string</returns>
        public virtual IEnumerable<string> PrintCommands()
        {
            foreach (var item in cmds.Values)
            {
                var args = item.CommandType.GenericTypeArguments;
                JsonArray arr = new JsonArray();
                for (int i = 0; i < args.Length; i++)
                {
                    arr.Add(args[i].Name);
                }
                JsonObject jobj = new JsonObject();
                jobj.Add(item.Alias, arr);
                yield return jobj.ToJsonString();
            }

        }

        /// <summary>
        /// Export the ModuleBase instance to json context
        /// </summary>
        /// <returns></returns>
        public JsonObject Export()
        {
            var setting = _config as IConfigurationSection;
            JsonObject jObj = setting == null ? new JsonObject() : JsonObject.Parse(setting.Value).AsObject();
            jObj.Add("Name", Alias);
            jObj.Add("Type", this.GetType().Name);
            var node = new JsonObject();
            node = _recorder.Export();
            if (node.Count > 0)
            {
                jObj.Add("Recorder", _recorder.Export());
            }
            node = _notifier.Export();
            if (node.Count > 0)
            {
                jObj.Add("Notifier", _notifier.Export());
            }
            node = _transceiver.Export();
            if (node.Count > 0)
            {
                jObj.Add("Transceiver", _transceiver.Export());
            }
            return jObj;
        }

        /// <summary>
        /// Export ModuleBase instance to json string format
        /// </summary>
        /// <param name="indented"></param>
        /// <returns></returns>
        public string Export(bool indented = true)
        {
            return Export().ToJsonString(new System.Text.Json.JsonSerializerOptions() { WriteIndented = indented });
        }

        public void Configure(object? option)
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
            cmds = ListMATSysCommands();
            _logger = LogManager.GetCurrentClassLogger();

        }
        public void InjectRecorder(IRecorder? recorder)
        {
            _recorder = recorder;
        }
        public void InjectTransceiver(ITransceiver? transceiver)
        {
            _transceiver = transceiver;


        }
        public void InjectNotifier(INotifier? notifier)
        {
            _notifier = notifier;

        }

        #endregion



        #region Private methods

        private Dictionary<string, MATSysCommandAttribute> ListMATSysCommands()
        {
            return GetType().GetMethods()
                .Where(x => x.GetCustomAttributes<MATSysCommandAttribute>(false).Count() > 0)
                .Select(x =>
                {
                    var cmd = x.GetCustomAttribute<MATSysCommandAttribute>()!;

                    //configure CommandType
                    cmd.ConfigureCommandType(x);

                    //configure MethodInvoker property
                    cmd.Invoker = MethodInvoker.Create(this, x);
                    return cmd;
                }).ToDictionary(x => x.Alias);
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
        /// Event when new request is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="commandObjectInJson"></param>
        /// <returns></returns>
        private string OnRequestReceived(object sender, string commandObjectInJson)
        {
            try
            {
                _logger?.Trace($"Command received: {commandObjectInJson}");
                var sp = commandObjectInJson.AsSpan();
                var end = sp.IndexOf(':');
                var start = sp.IndexOf('\"');
                var item = cmds[sp.Slice(start + 1, end - start - 2).ToString()];
                var cmd = CommandBase.Deserialize(commandObjectInJson, item.CommandType!);
                return Execute(cmd);
            }
            catch (KeyNotFoundException ex)
            {
                _logger?.Warn(ex);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_notFound, ex, commandObjectInJson);
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger?.Warn(ex);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_serDesError, ex, commandObjectInJson);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_execError, ex, commandObjectInJson);
            }
        }

        #endregion
    }

}