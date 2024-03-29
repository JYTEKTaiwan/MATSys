﻿using MATSys.Commands;
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
        private readonly ILogger? _logger;
        private readonly ITransceiver _transceiver;
        private readonly IRecorder _recorder;
        private readonly INotifier _notifier;

        /// <summary>
        /// private field to use
        /// </summary>
        private volatile bool _isRunning = false;
        private readonly Dictionary<string, MATSysCommandAttribute> cmds;
        private object? _config;
        private IServiceProvider _provider;
        #endregion

        /// <summary>
        /// Running status of ModuleBase instance
        /// </summary>
        public bool IsRunning => _isRunning;
        /// <summary>
        /// Name of the ModuleBase instance
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Instance of current ModuleBase instance
        /// </summary>
        public IModule Base => this;

        /// <summary>
        /// Collection of IModules created by ModuleHubBackgroundService
        /// </summary>
        public Dictionary<string, IModule> LocalPeers { get; set; } = new Dictionary<string, IModule>();

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

        IServiceProvider IModule.Provider { get; set; }
        /// <summary>
        /// Event when new data is generated inside ModuleBase instance
        /// </summary>
        public event IModule.NewDataReady? OnDataReady;

        /// <summary>
        /// ctor of ModuleBase class. 
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="transceiver">transceiver instance</param>
        /// <param name="notifier">notifier instance</param>
        /// <param name="recorder">recorder instance</param>
        /// <param name="aliasName">alias name</param>
        public ModuleBase(object? configuration, ITransceiver? transceiver, INotifier? notifier, IRecorder? recorder, string aliasName = "")
        {
            Name = string.IsNullOrEmpty(aliasName) ? $"{GetType().Name}_{GetHashCode().ToString("X2")}" : aliasName;
            if (LogManager.Configuration != null)
            {
                _logger = LogManager.GetLogger(Name);
            }
            _config = configuration;
            LoadAndSetup(configuration);
            _recorder = InjectRecorder(recorder);
            _notifier = InjectNotifier(notifier);
            _transceiver = InjectTransceiver(transceiver);
            cmds = ListMATSysCommands();
            _logger?.Info($"{Name} base class initialization is completed");
        }

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
        public virtual void StartService(CancellationToken token)
        {
            if (!_isRunning)
            {
                try
                {
                    _logger?.Trace($"Starts the {_recorder.Name}");
                    _recorder.StartService(token);
                    _logger?.Trace($"Starts the {_notifier.Name}");
                    _notifier.StartService(token);
                    _logger?.Trace($"Starts the {_transceiver.Name}");
                    _transceiver.StartService(token);
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
        public virtual void StopService()
        {
            try
            {
                if (_isRunning)
                {
                    _logger?.Trace($"Stops the {_transceiver.Name}");
                    _transceiver.StopService();

                    _logger?.Trace($"Stops the {_recorder.Name}");
                    _recorder.StopService();

                    _logger?.Trace($"Stops the {_notifier.Name}");
                    _notifier.StopService();
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
            jObj.Add("Name", Name);
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
        /// load the configuration object
        /// </summary>
        /// <param name="option">parameter object</param>
        /// <returns>configuration instance (null if <paramref name="option"/> is null</returns>
        private void LoadAndSetup(object? option)
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
        }
        /// <summary>
        /// Inject the IRecorder instance
        /// </summary>
        /// <param name="recorder">recorder instance</param>
        /// <returns>IRecorder instance (return EmptyRecorder if <paramref name="recorder"/> is null</returns>
        private IRecorder InjectRecorder(IRecorder? recorder)
        {
            if (recorder == null)
            {
                var _recorder = new EmptyRecorder();
                _logger?.Info($"Null reference is detected, {_recorder.Name} is injected");
                return _recorder;
            }
            else
            {
                _logger?.Info($"{recorder.Name} is injected");
                return recorder;
            }

        }
        /// <summary>
        /// Inject the ITransceiver instance
        /// </summary>
        /// <param name="transceiver">ITransceiver instance</param>
        /// <returns>ITransceiver instance (return EmptyTransceiver if <paramref name="transceiver"/> is null</returns>
        private ITransceiver InjectTransceiver(ITransceiver? transceiver)
        {
            if (transceiver == null)
            {
                var _transceiver = new EmptyTransceiver();
                _transceiver.OnNewRequest += OnRequestReceived;
                _logger?.Info($"Null reference is detected, {_transceiver.Name} is injected");
                return _transceiver;
            }
            else
            {
                transceiver.OnNewRequest += OnRequestReceived;
                _logger?.Info($"{transceiver.Name} is injected");
                return transceiver;
            }


        }
        /// <summary>
        /// Inject the INotifier instance
        /// </summary>
        /// <param name="notifier">INotifier instance</param>
        /// <returns>INotifier instance (return EmptyNotifier if <paramref name="notifier"/> is null</returns>
        private INotifier InjectNotifier(INotifier? notifier)
        {
            if (notifier == null)
            {
                var _notifier = new EmptyNotifier();
                _notifier.OnNotify += OnNewDateGenerated;
                _logger?.Info($"Null reference is detected, {_notifier.Name} is injected");
                return _notifier;
            }
            else
            {
                notifier.OnNotify += OnNewDateGenerated;
                _logger?.Info($"{notifier.Name} is injected");
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