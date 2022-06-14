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
        private const string key_dataRecorder = "DataRecorder";
        private const string key_publisher = "DataBus";
        private const string key_server = "CommandServer";

        private readonly Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        private readonly ILogger _logger;
        private readonly ICommandServer _server;
        private readonly IDataRecorder _dataRecorder;
        private readonly IDataBus _dataBus;
        private volatile bool isRunning = false;

        public event IDevice.NewDataReady? OnDataReady;

        ILogger IDevice.Logger => _logger;
        IDataRecorder IDevice.DataRecorder => _dataRecorder;
        ICommandServer IDevice.Server => _server;
        IDataBus IDevice.DataBus => _dataBus;
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
                _dataRecorder = services.GetRequiredService<IDataRecorderFactory>().CreateRecorder(section.GetSection(key_dataRecorder));
                _logger.Trace($"{_dataRecorder.Name} is injected");
                _dataBus = services.GetRequiredService<IDataBusFactory>().CreatePublisher(section.GetSection(key_publisher));
                _dataBus.OnNewDataReadyEvent += NewDataReady;
                _logger.Trace($"{_dataBus.Name} is injected");
                _server = services.GetRequiredService<ICommandServerFactory>().CreateCommandStream(section.GetSection(key_server));
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

        public DeviceBase(ICommandServer server, IDataBus bus, IDataRecorder recorder)
        {
            Name = $"{this.GetType().Name}_{this.GetHashCode().ToString("X2")}";
            _logger = NLog.LogManager.GetLogger(Name);

            if (recorder == null)
            {
                _dataRecorder = new EmptyDataRecorder();
                _logger.Trace($"Null reference is detected, {_dataRecorder.Name} is injected");
            }
            else
            {
                _dataRecorder = recorder;
                _logger.Trace($"{_dataRecorder.Name} is injected");
            }

            if (bus == null)
            {
                _dataBus = new EmptyDataBus();
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
                _server = new EmptyCommandServer();
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
                return x.GetCustomAttributes<MATSysCommandAttribute>(false).Count() > 0;
            }).ToArray();
            return methodlist.ToDictionary(x => x.GetCustomAttribute<MATSysCommandAttribute>()!.Name);
        }

        private string OnCommandDataReady(object sender, string commandObjectInJson)
        {
            try
            {
                var answer = "";
                _logger.Trace($"OnDataReady event fired");
                _logger.Debug($"New command object string is received: {commandObjectInJson}");
                var parsedName = commandObjectInJson.Split(new string[] { "MethodName\"" }, StringSplitOptions.RemoveEmptyEntries)[1].Split('\"')[1];
                var method = methods[parsedName];
                var att = method.GetCustomAttribute<MATSysCommandAttribute>();
                var cmd = JsonConvert.DeserializeObject(commandObjectInJson, att!.CommandType) as ICommand;
                _logger.Debug($"Converted to command object successfully: {cmd!.MethodName}");

                answer = Execute(cmd);

                _logger.Debug($"Command object execution completed with return value {answer}");
                _logger.Info($"Command [{cmd.MethodName}] is executed successfully");
                return answer;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new Exception($"Execution of command ready event failed", ex);
            }
        }

        public virtual Task StartServiceAsync(CancellationToken token)
        {
            if (!isRunning)
            {
                return Task.Run(() =>
                {
                    try
                    {
                        _logger.Trace("Starts the DataRecorder");
                        var t1 = _dataRecorder.StartServiceAsync(token); ;

                        _logger.Trace("Starts the Publisher");
                        var t2 = _dataBus.StartServiceAsync(token); ;

                        _logger.Trace("Starts the CommandServer");
                        var t3 = _server.StartServiceAsync(token); ;

                        Task.WaitAll(t1, t2, t3);

                        _logger.Info("Starts service");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        throw new Exception($"RunAsync failed", ex);
                    }
                });
            }
            else
            {
                return Task.CompletedTask;
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
            try
            {
                var method = methods[cmd.MethodName];
                var result = method.Invoke(this, cmd.GetParameters())!;
                return cmd.ConvertResultToString(result)!;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new Exception($"Execute command failed", ex);
            }
        }

        public abstract void Load(IConfigurationSection section);

        public virtual IEnumerable<string> PrintCommands()
        {
            var cmds = GetType().GetMethods().Where(x => x.GetCustomAttributes<MATSysCommandAttribute>().Count() > 0);
            foreach (var item in cmds)
            {
                yield return item.GetCustomAttribute<MATSysCommandAttribute>()!.GetJsonString();
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