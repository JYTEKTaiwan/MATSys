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
    public abstract class DeviceBase : IDevice
    {
        private const string key_dataRecorder = "Recorder";
        private const string key_publisher = "Notifier";
        private const string key_server = "Transceiver";
        public const string cmd_notFound = "NOTFOUND";
        public const string cmd_execError = "EXEC_ERROR";
        private readonly Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        private readonly ILogger _logger;
        private readonly ITransceiver _server;
        private readonly IRecorder _dataRecorder;
        private readonly INotifier _dataBus;
        public volatile bool isRunning = false;

        public bool IsRunning => isRunning;
        public event IDevice.NewDataReady? OnDataReady;

        ILogger IDevice.Logger => _logger;
        IRecorder IDevice.DataRecorder => _dataRecorder;
        ITransceiver IDevice.Server => _server;
        INotifier IDevice.DataBus => _dataBus;
        public string Name { get; }
        public IDevice Instance => this;

        public DeviceBase(IServiceProvider services, string configurationKey)
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
                _dataRecorder = services.GetRequiredService<IRecorderFactory>().CreateRecorder(section.GetSection(key_dataRecorder));
                _logger.Trace($"{_dataRecorder.Name} is injected");
                _dataBus = services.GetRequiredService<INotifierFactory>().CreateNotifier(section.GetSection(key_publisher));
                _dataBus.OnNewDataReadyEvent += NewDataReady;
                _logger.Trace($"{_dataBus.Name} is injected");
                _server = services.GetRequiredService<ITransceiverFactory>().CreateTransceiver(section.GetSection(key_server));
                _logger.Trace($"{_server.Name} is injected");
                _server.OnCommandReady += OnCommandDataReady; ;
                methods = ParseSupportedMethods();
                _logger.Info($"{Name} base class initialization is completed");
            }
            catch (Exception ex)
            {
                _logger?.Error(ex.Message);
                throw new Exception($"Initialization of DeviceBase failed", ex);
            }
        }

        public DeviceBase(ITransceiver server, INotifier bus, IRecorder recorder)
        {
            Name = $"{this.GetType().Name}_{this.GetHashCode().ToString("X2")}";
            _logger = NLog.LogManager.GetLogger(Name);

            if (recorder == null)
            {
                _dataRecorder = new EmptyRecorder();
                _logger.Trace($"Null reference is detected, {_dataRecorder.Name} is injected");
            }
            else
            {
                _dataRecorder = recorder;
                _logger.Trace($"{_dataRecorder.Name} is injected");
            }

            if (bus == null)
            {
                _dataBus = new EmptyNotifier();
                _logger.Trace($"Null reference is detected, {_dataBus.Name} is injected");
            }
            else
            {
                _dataBus = bus;
                _logger.Trace($"{_dataBus.Name} is injected");
            }

            _dataBus.OnNewDataReadyEvent += NewDataReady;

            if (server == null)
            {
                _server = new EmptyTransceiver();
                _logger.Trace($"Null reference is detected, {_server.Name} is injected");
            }
            else
            {
                _server = server;
                _logger.Trace($"{_server.Name} is injected");
            }

            _server.OnCommandReady += OnCommandDataReady; ;
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
                return x.GetCustomAttributes<PrortotypeAttribute>(false).Count() > 0;
            }).ToArray();
            return methodlist.ToDictionary(x => x.GetCustomAttribute<PrortotypeAttribute>()!.Name);
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
                    var att = method.GetCustomAttribute<PrortotypeAttribute>();
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
                    _dataRecorder.StartService(token);

                    _logger.Trace("Starts the Publisher");
                    _dataBus.StartService(token);

                    _logger.Trace("Starts the CommandServer");
                    _server.StartService(token);

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
                    _server.StopService();

                    _logger.Trace("Stops the DataRecorder");
                    _dataRecorder.StopService();

                    _logger.Trace("Stops the Publisher");
                    _dataBus.StopService();
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

        public virtual IEnumerable<string> PrintCommands()
        {
            var cmds = GetType().GetMethods().Where(x => x.GetCustomAttributes<PrortotypeAttribute>().Count() > 0);
            foreach (var item in cmds)
            {
                yield return item.GetCustomAttribute<PrortotypeAttribute>()!.GetJsonString();
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