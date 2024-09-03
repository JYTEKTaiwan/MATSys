#if NET8_0_OR_GREATER
using System.Collections.Frozen;
using System.Runtime.InteropServices;

#else
using System.Collections.ObjectModel;
#endif
using MATSys.Commands;
using MATSys.Plugins;
using MATSys.Utilities;

namespace MATSys
{
    /// <summary>
    /// Abstract class equipped with ITransceiver, IRecorder, INotifier plugins
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        #region Private Fields
        private NLog.ILogger? _logger;
        private ITransceiver _transceiver = new EmptyTransceiver();
        private IRecorder _recorder = new EmptyRecorder();
        private INotifier _notifier = new EmptyNotifier();
        private volatile bool _isRunning = false;
        private IServiceProvider _provider;
        private bool _disposedValue = false;
#if NET8_0_OR_GREATER
        private FrozenDictionary<string, MATSysContext> cmds = null!;

#elif NET6_0_OR_GREATER || NETSTANDARD2_0
        private ReadOnlyDictionary<string, MATSysContext> cmds = null!;

#else
        private Dictionary<string, MATSysContext> cmds = null;

#endif

        #endregion

        #region Public Properties

        public abstract object Configuration { get;  set; }

        /// <summary>
        /// Running status of ModuleBase instance
        /// </summary>
        public bool IsRunning => _isRunning;
        /// <summary>
        /// Name of the ModuleBase instance
        /// </summary>
        public string Alias { get; set; } = "";

        /// <summary>
        /// Instance of current ModuleBase instance
        /// </summary>
        public IModule Base => this;
        public IServiceProvider Provider => _provider;
        /// <summary>
        /// ILogger instance
        /// </summary>
        NLog.ILogger? IModule.Logger => _logger;

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

        public event ServiceDisposed Disposed;
        public event ServiceExceptionFired? ExceptionFired;
        /* Unmerged change from project 'MATSys (net35)'
        Before:
                #endregion

                #region Constructor
        After:
                #endregion

                #region Constructor
        */
        #endregion

        #region Constructor
        protected ModuleBase()
        {
#if NET8_0_OR_GREATER
            cmds = ListMATSysCommands().ToFrozenDictionary();
#elif NET6_0_OR_GREATER || NETSTANDARD2_0
            cmds = new ReadOnlyDictionary<string, MATSysContext>(ListMATSysCommands());
#else
            cmds = new Dictionary<string, MATSysContext>(ListMATSysCommands());
#endif
            _logger = LogManager.GetCurrentClassLogger();

        }


        #endregion

        #region Public Methods

#if NET6_0_OR_GREATER || NETSTANDARD2_0
        /// <summary>
        /// Asynchronously execute the assigned command
        /// </summary>
        /// <param name="cmd">ICommand instance</param>
        /// <returns>Serialized response</returns>
        public async Task<string> ExecuteAsync(ICommand cmd)
        {
            return await Task.Run(async () =>
            {
                var response = await ExecuteRawAsync(cmd);
                return cmd.ConvertResultToString(response)!;
            });
        }
        public async Task<object> ExecuteRawAsync(ICommand cmd)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    _logger?.Trace($"Command is ready to executed {cmd.MethodName}");
                    var invoker = cmds[cmd.MethodName].Invoker;
                    if (invoker == null)
                    {
                        throw new NullReferenceException("invoker is null");
                    }
                    var result = await invoker.InvokeAsync(cmd.GetParameters());
                    _logger?.Info($"Command [{cmd.MethodName}] is executed successfully");
                    return result!;
                }
                catch (KeyNotFoundException ex)
                {
                    _logger?.Warn(ex);
                    ExceptionFired?.Invoke(this, ex);
                    return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_notFound, ex, cmd);
                }
                catch (Exception ex)
                {
                    _logger?.Warn(ex);
                    ExceptionFired?.Invoke(this, ex);
                    return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_execError, ex, cmd);
                }
            });
        }

        public async Task<string> ExecuteAsync(string methodName, params object[] parameters)
        {
            return await Task.Run(async () =>
            {
                var result = await ExecuteRawAsync(methodName, parameters)!;
                var response = Utilities.Serializer.Serialize(result, false)!;
                return response!;
            });

        }
        public async Task<object> ExecuteRawAsync(string methodName, params object[] parameters)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    _logger?.Trace($"Command is ready to executed {methodName}");
                    var invoker = cmds[methodName].Invoker;
                    if (invoker == null)
                    {
                        throw new NullReferenceException("invoker is null");
                    }
                    var result = await invoker.InvokeAsync(parameters);
                    _logger?.Info($"Command [{methodName}] is executed successfully");
                    return result!;
                }
                catch (KeyNotFoundException ex)
                {
                    _logger?.Warn(ex);
                    ExceptionFired?.Invoke(this, ex);
                    return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_notFound, ex, methodName);
                }
                catch (AggregateException ex)
                {
                    _logger?.Warn(ex.Message);
                    ExceptionFired?.Invoke(this, ex);
                    return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_execError, ex, methodName);
                }
                catch (Exception ex)
                {
                    _logger?.Warn(ex);
                    ExceptionFired?.Invoke(this, ex);
                    return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_execError, ex, methodName);
                }
            });

        }

#endif
        /// <summary>
        /// Execute the incoming command object
        /// </summary>
        /// <param name="cmd">ICommand instance</param>
        /// <returns>reponse after executing the commnad</returns>
        public string Execute(ICommand cmd)
        {            
#if NET6_0_OR_GREATER || NETSTANDARD2_0
            return ExecuteAsync(cmd).Result;
#else
            return Execute_Net35(cmd);
#endif
        }
        public string Execute(string methodName, params object[] parameters)
        {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
            return ExecuteAsync(methodName, parameters).Result;
#else
            return Execute_Net35(methodName, parameters);
#endif
        }
        public void Execute(string cmdInJson, out string response) => response = OnRequestReceived(this, cmdInJson);

        public object ExecuteRaw(ICommand cmd)
        {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
            return ExecuteRawAsync(cmd).Result;
#else
            return ExecuteRaw_Net35(cmd);
#endif
        }

        public object ExecuteRaw(string methodName, params object[] parameters)
        {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
            return ExecuteRawAsync(methodName, parameters).Result;
#else
            return ExecuteRaw_Net35(methodName, parameters);
#endif
        }


        public void ExecuteRaw(string cmdInJson, out object response) => response = OnRequestReceived(cmdInJson);

        /// <summary>
        /// List all methods in simplied string format 
        /// </summary>
        /// <returns>collection of string</returns>
        public virtual IEnumerable<string> PrintCommands()
        {
            foreach (var item in cmds.Values)
            {

#if NET6_0_OR_GREATER || NETSTANDARD2_0
                var args = item.CommandType!.GenericTypeArguments;
                System.Text.Json.Nodes.JsonArray arr = new System.Text.Json.Nodes.JsonArray();
                for (int i = 0; i < args.Length; i++)
                {
                    arr.Add(args[i].Name);
                }
                System.Text.Json.Nodes.JsonObject jobj = new System.Text.Json.Nodes.JsonObject();
                jobj.Add(item.MethodName, arr);
                yield return jobj.ToJsonString();
#else
                var args = item.CommandType!.GetGenericArguments();
                Newtonsoft.Json.Linq.JArray arr = new Newtonsoft.Json.Linq.JArray();
                for (int i = 0; i < args.Length; i++)
                {
                    arr.Add(args[i].Name);
                }
                Newtonsoft.Json.Linq.JObject jobj = new Newtonsoft.Json.Linq.JObject();
                jobj.Add(item.MethodName, arr);
                yield return jobj.ToString();
#endif
            }

        }
#if NET6_0_OR_GREATER || NETSTANDARD2_0
        /// <summary>
        /// Export the ModuleBase instance to json context
        /// </summary>
        /// <returns></returns>
        public System.Text.Json.Nodes.JsonObject Export()
        {
            System.Text.Json.Nodes.JsonObject jObj = new System.Text.Json.Nodes.JsonObject();

            jObj.Add("Name", Alias);
            jObj.Add("Type", this.GetType().Name);
            if (_recorder.Export().Count > 0) jObj.Add("Recorder", _recorder.Export());

            if (_notifier.Export().Count > 0) jObj.Add("Notifier", _notifier.Export());

            if (_transceiver.Export().Count > 0) jObj.Add("Transceiver", _transceiver.Export());

            return jObj;
        }

#else
        /// <summary>
        /// Export the ModuleBase instance to json context
        /// </summary>
        /// <returns></returns>
        public Newtonsoft.Json.Linq.JObject Export()
        {
            Newtonsoft.Json.Linq.JObject jObj = new Newtonsoft.Json.Linq.JObject();

            jObj.Add("Name", Alias);
            jObj.Add("Type", this.GetType().Name);

            if (_recorder.Export().Count > 0) jObj.Add("Recorder", _recorder.Export());

            if (_notifier.Export().Count > 0) jObj.Add("Notifier", _notifier.Export());

            if (_transceiver.Export().Count > 0) jObj.Add("Transceiver", _transceiver.Export());

            return jObj;
        }

#endif
        /// <summary>
        /// Export ModuleBase instance to json string format
        /// </summary>
        /// <param name="indented"></param>
        /// <returns></returns>
        public string Export(bool indented = true)
        {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
            return Export().ToJsonString(new System.Text.Json.JsonSerializerOptions() { WriteIndented = indented });
#else
            if (indented) return Export().ToString(Newtonsoft.Json.Formatting.Indented);
            else return Export().ToString(Newtonsoft.Json.Formatting.None);

#endif
        }
        /// <summary>
        /// Configurae IModule instance with object 
        /// </summary>
        /// <param name="option">configuration object</param>
        public virtual void Configure() { }
        /// <summary>
        /// Inject the IRecorder instance into IModule instance
        /// </summary>
        /// <param name="recorder">IRecorder instance</param>
        public void InjectPlugin(IRecorder? recorder) => _recorder = recorder == null ? _recorder : recorder;

        /// <summary>
        /// Inject the ITransceiver instance into IModule instance
        /// </summary>
        /// <param name="transceiver">ITransceiver instance</param>
        public void InjectPlugin(ITransceiver? transceiver)
        {
            _transceiver = transceiver == null ? _transceiver : transceiver;
            _transceiver.RequestReceived += OnRequestReceived;

        }


        /// <summary>
        /// Inject the INotifier instance into IModule instance
        /// </summary>
        /// <param name="notifier">INotifier instance </param>
        public void InjectPlugin(INotifier? notifier) => _notifier = notifier == null ? _notifier : notifier;

        public void SetProvider(IServiceProvider provider)
        {
            _provider = provider;
        }
        #endregion

        #region Private methods
#if NET35 || NET462

        private string Execute_Net35(ICommand cmd)
        {
            return  cmd.ConvertResultToString(ExecuteRaw_Net35(cmd))!;

        }
        private object ExecuteRaw_Net35(ICommand cmd)
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
                _logger?.Info($"Command [{cmd.MethodName}] is executed successfully");
                return result;
            }
            catch (KeyNotFoundException ex)
            {
                _logger?.Warn(ex);
                ExceptionFired?.Invoke(this, ex);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_notFound, ex, cmd);
            }
            catch (Exception ex)
            {
                _logger?.Warn(ex);
                ExceptionFired?.Invoke(this, ex);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_execError, ex, cmd);
            }

        }

        private string Execute_Net35(string methodName, params object[] parameters)
        {
            return Serializer.Serialize(ExecuteRaw_Net35(methodName,parameters),false)!;


        }
        private object ExecuteRaw_Net35(string methodName, params object[] parameters)
        {
            try
            {
                var invoker = cmds[methodName].Invoker;
                if (invoker == null)
                {
                    throw new NullReferenceException("invoker is null");
                }
                var result = invoker.Invoke(parameters);
                return result!;
            }
            catch (KeyNotFoundException ex)
            {
                _logger?.Warn(ex);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_notFound, ex, methodName);
            }
            catch (Exception ex)
            {
                _logger?.Warn(ex);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_execError, ex, methodName);
            }


        }

#endif
        private Dictionary<string, MATSysContext> ListMATSysCommands()
        {
#if NET6_0_OR_GREATER || NETSTANDARD2_0||NET462
            return GetType().GetMethods()
                           .Where(x => x.GetCustomAttributes<MATSysCommandAttribute>().Count() > 0)
                           .Select(x =>
                           {
                               return MATSysContext.Create(this, x);
                           }).ToDictionary(x => x.MethodName);

#elif NET35
            return GetType().GetMethods()
               .Where(x => x.GetCustomAttributes(typeof(MATSysCommandAttribute), true).Count() > 0)
               .Select(x =>
               {
                   return MATSysContext.Create(this, x);
               }).ToDictionary(x => x.MethodName);

#endif
        }
        /// <summary>
        /// Event when new request is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="commandObjectInJson"></param>
        /// <returns></returns>
        private string OnRequestReceived(object sender, string commandObjectInJson)
        {
            return Serializer.Serialize(OnRequestReceived(commandObjectInJson), false);
        }

        private object OnRequestReceived(string commandObjectInJson)
        {
            try
            {
                _logger?.Trace($"Command received: {commandObjectInJson}");
#if NET6_0_OR_GREATER || NETSTANDARD2_0
                var sp = commandObjectInJson.AsSpan();
                var end = sp.IndexOf(':');
                var start = sp.IndexOf('\"');
                var item = cmds[sp.Slice(start + 1, end - start - 2).ToString()];
#else
                var sp = commandObjectInJson;
                var end = sp.IndexOf(':');
                var start = sp.IndexOf('\"');
                var item = cmds[sp.Substring(start + 1, end - start - 2).ToString()];
#endif

                var cmd = CommandBase.Deserialize(commandObjectInJson, item.CommandType!);
                return ExecuteRaw(cmd);
            }
            catch (KeyNotFoundException ex)
            {
                _logger?.Warn(ex);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_notFound, ex, commandObjectInJson);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_execError, ex, commandObjectInJson);
            }

        }

        /// <summary>
        /// Dispose the instance and call GC
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
        }
        // Protected implementation of Dispose pattern.
        public virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _notifier.Dispose();
                    _transceiver.Dispose();
                    _recorder.Dispose();
                    _isRunning = false;
                    Disposed?.Invoke(this, EventArgs.Empty);
                }
                _disposedValue = true;
            }
        }
        #endregion
    }

}
