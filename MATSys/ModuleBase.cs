#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#else
using System.Collections.ObjectModel;
#endif
using MATSys.Commands;
using MATSys.Plugins;

namespace MATSys
{
    /// <summary>
    /// Abstract class equipped with ITransceiver, IRecorder, INotifier plugins
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        #region Private Fields

        /// <summary>
        /// section key for recorder in specific section of module
        /// </summary>
        private const string key_recorder = "Recorder";

        /// <summary>
        /// section key for notifier in specific section of module
        /// </summary>
        private const string key_notifier = "Notifier";
        /// <summary>
        /// section key for transceiver in specific section of module
        /// </summary>
        private const string key_transceiver = "Transceiver";
        private NLog.ILogger? _logger;
        private ITransceiver _transceiver = new EmptyTransceiver();
        private IRecorder _recorder = new EmptyRecorder();
        private INotifier _notifier = new EmptyNotifier();
        private volatile bool _isRunning = false;

#if NET8_0_OR_GREATER
        private FrozenDictionary<string, MATSysContext> cmds = null!;
#elif NET6_0_OR_GREATER || NETSTANDARD2_0
        private ReadOnlyDictionary<string, MATSysContext> cmds = null!;
#else
        private Dictionary<string, MATSysContext> cmds = null!;
#endif

        private IConfigurationSection? _config;
        #endregion

        #region Public Properties
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


        /// <summary>
        /// Event when object is disposed
        /// </summary>
        public event EventHandler? IsDisposed;

        #endregion

        #region Constructor
        // public ModuleBase(IServiceProvider provider)
        // {            
        //     Alias = this.GetType().GetConstructors().Where(x=>x.GetCustomAttribute<IDAttribute>()!=null).FirstOrDefault().GetCustomAttribute<IDAttribute>().ID;
        //     var _transceiverFactory = provider.GetRequiredService<ITransceiverFactory>();
        //     var _notifierFactory = provider.GetRequiredService<INotifierFactory>();
        //     var _recorderFactory = provider.GetRequiredService<IRecorderFactory>();

        //     Configuration = provider.GetRequiredService<IConfiguration>().GetSection("MATSys:Modules").GetChildren().First(x => x["Alias"] == Alias);
        //     // var typeString = Configuration.GetValue<string>("Type");
        //     // string extAssemblyPath = Configuration.GetValue<string>("AssemblyPath"); //Get the assemblypath string of Type in json section
        //     // var t = TypeParser.SearchType(typeString, extAssemblyPath);

        //     _transceiver = _transceiverFactory.CreateTransceiver(Configuration.GetSection(key_transceiver));
        //     _notifier = _notifierFactory.CreateNotifier(Configuration.GetSection(key_notifier));
        //     _recorder = _recorderFactory.CreateRecorder(Configuration.GetSection(key_recorder));
        //     cmds = ListMATSysCommands();
        // }
        #endregion


        /* Unmerged change from project 'MATSys (net35)'
        Before:
                #region Public Methods

                /// <summary>
        After:
                #region Public Methods

                /// <summary>
        */
        #region Public Methods

        /// <summary>
        /// Load condfiguration from custom instance
        /// </summary>
        /// <param name="configuration"></param>
        public virtual void Load(object configuration)
        {
            //do nothing(let user to assign the logic)
        }



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
                try
                {
                    _logger?.Trace($"Command is ready to executed {cmd.MethodName}");
                    var invoker = cmds[cmd.MethodName].Invoker;
                    if (invoker == null)
                    {
                        throw new NullReferenceException("invoker is null");
                    }
                    var result = await invoker.InvokeAsync(cmd.GetParameters());
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
                catch (System.Text.Json.JsonException ex)
                {
                    _logger?.Warn(ex);
                    return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_serDesError, ex, cmd);
                }
                catch (Exception ex)
                {
                    _logger?.Warn(ex);
                    return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_execError, ex, cmd);
                }
            });
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
                return ExecuteAsync(cmd).Result;
            }
            catch (AggregateException ex)
            {
                _logger?.Warn(ex.Message);
                return ExceptionHandler.PrintMessage(ExceptionHandler.cmd_execError, ex, cmd);
            }
        }
#elif NET35

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
#endif
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
#if NET6_0_OR_GREATER || NETSTANDARD2_0
            foreach (var item in cmds.Values)
            {
                var args = item.CommandType!.GenericTypeArguments;
                System.Text.Json.Nodes.JsonArray arr = new System.Text.Json.Nodes.JsonArray();
                for (int i = 0; i < args.Length; i++)
                {
                    arr.Add(args[i].Name);
                }
                System.Text.Json.Nodes.JsonObject jobj = new System.Text.Json.Nodes.JsonObject();
                jobj.Add(item.MethodName, arr);
                yield return jobj.ToJsonString();
            }
#elif NET35
            foreach (var item in cmds.Values)
            {
                var args = item.CommandType!.GetGenericArguments();
                Newtonsoft.Json.Linq.JArray arr = new Newtonsoft.Json.Linq.JArray();
                for (int i = 0; i < args.Length; i++)
                {
                    arr.Add(args[i].Name);
                }
                Newtonsoft.Json.Linq.JObject jobj = new Newtonsoft.Json.Linq.JObject();
                jobj.Add(item.MethodName, arr);
                yield return jobj.ToString();
            }
#endif

        }
#if NET6_0_OR_GREATER||NETSTANDARD2_0
        /// <summary>
        /// Export the ModuleBase instance to json context
        /// </summary>
        /// <returns></returns>
        public System.Text.Json.Nodes.JsonObject Export()
        {
            System.Text.Json.Nodes.JsonObject jObj = new System.Text.Json.Nodes.JsonObject();

            jObj.Add("Name", Alias);
            jObj.Add("Type", this.GetType().Name);
            var node = new System.Text.Json.Nodes.JsonObject();
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

#elif NET35
        /// <summary>
        /// Export the ModuleBase instance to json context
        /// </summary>
        /// <returns></returns>
        public Newtonsoft.Json.Linq.JObject Export()
        {
            Newtonsoft.Json.Linq.JObject jObj = new Newtonsoft.Json.Linq.JObject();

            jObj.Add("Name", Alias);
            jObj.Add("Type", this.GetType().Name);
            var node = new Newtonsoft.Json.Linq.JObject();
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
#elif NET35
            if (indented) return Export().ToString(Newtonsoft.Json.Formatting.Indented);
            else return Export().ToString(Newtonsoft.Json.Formatting.None);

#else
#endif
        }
        /// <summary>
        /// Configurae IModule instance with object 
        /// </summary>
        /// <param name="option">configuration object</param>
        public void Configure(object? option)
        {
            if (option != null)
            {
                if (typeof(IConfigurationSection).IsAssignableFrom(option.GetType()))
                {
                    _config = (IConfigurationSection)option;
                    Load(_config);
                }
                else
                    Load(option);
            }
#if NET8_0_OR_GREATER
            cmds = ListMATSysCommands().ToFrozenDictionary();
#elif NET6_0_OR_GREATER || NETSTANDARD2_0
            cmds = new ReadOnlyDictionary<string, MATSysContext>(ListMATSysCommands());
#else
            cmds = new Dictionary<string, MATSysContext>(ListMATSysCommands());
#endif
            _logger = LogManager.GetCurrentClassLogger();

        }
        /// <summary>
        /// Inject the IRecorder instance into IModule instance
        /// </summary>
        /// <param name="recorder">IRecorder instance</param>
        public void InjectRecorder(IRecorder? recorder)
        {
            _recorder = recorder!;
        }
        /// <summary>
        /// Inject the ITransceiver instance into IModule instance
        /// </summary>
        /// <param name="transceiver">ITransceiver instance</param>
        public void InjectTransceiver(ITransceiver? transceiver)
        {
            _transceiver = transceiver!;


        }
        /// <summary>
        /// Inject the INotifier instance into IModule instance
        /// </summary>
        /// <param name="notifier">INotifier instance </param>
        public void InjectNotifier(INotifier? notifier)
        {
            _notifier = notifier!;

        }

        #endregion

        #region Private methods

        private Dictionary<string, MATSysContext> ListMATSysCommands()
        {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
            return GetType().GetMethods()
               .Where(x => x.GetCustomAttributes<MATSysCommandAttribute>().Count() > 0)
               .Select(x =>
               {
                   return MATSysContext.Create(this, x);
               }).ToDictionary(x => x.MethodName);

#elif NET35
            return GetType().GetMethods()
                           .Where(x => x.GetCustomAttributes(typeof(MATSysCommandAttribute), false).Count() > 0)
                           .Select(x =>
                           {
                               return MATSysContext.Create(this, x);
                           }).ToDictionary(x => x.MethodName);

#endif

            ;
        }
#if NET6_0_OR_GREATER || NETSTANDARD2_0
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
            _notifier.Dispose();
            _transceiver.Dispose();
            _recorder.Dispose();
            _isRunning = false;
            IsDisposed?.Invoke(this, null!);
            GC.Collect();
        }


#elif NET35
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
                var sp = commandObjectInJson;
                var end = sp.IndexOf(':');
                var start = sp.IndexOf('\"');
                var item = cmds[sp.Substring(start + 1, end - start - 2).ToString()];
                var cmd = CommandBase.Deserialize(commandObjectInJson, item.CommandType!);
                return Execute(cmd);
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
            _notifier.Dispose();
            _transceiver.Dispose();
            _recorder.Dispose();
            _isRunning = false;
            IsDisposed?.Invoke(this, null!);
            GC.Collect();
        }


#endif
        #endregion
    }

    internal interface IConfigurationSection
    {
    }
}
namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute { }
}