using NUnit.Framework;
using MATSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Text;
using MATSys.Commands;
using System.Text.Json;
using MATSys.Plugins;
using System.Security.Cryptography;

namespace MATSys.Tests
{
    [TestFixture()]
    public class ModuleBaseTests
    {
        internal class TestModule : ModuleBase
        {
            public override object Configuration { get; set; }
            [MATSysCommand]
            public bool ReturnFalse() => false;

            [MATSysCommand]
            public void AsyncTestMethod(int delay)
            {
                Thread.Sleep(delay);
            }

            [MATSysCommand]
            public void ThrowExceptionMethod()
            {
                throw new MethodAccessException();
            }


        }

        private IModule _module;
        private Exception _ex = new Exception();
        [SetUp]
        public void ModuleBaseSetup()
        {
            _module = new TestModule();
            _module.ExceptionFired += CatchExceptionFromEvent;
        }
        [TearDown]
        public void ModuleBaseTearDown()
        {
            _module.Dispose();
        }

        private void CatchExceptionFromEvent(object sender, Exception ex)
        {
            _ex = ex;
        }
        #region Execution Test

        [Test]
        [Category("Execution")]
        public void ExceptionFiredTest()
        {
            _ex = null;
            var cmd = CommandBase.Create("ThrowExceptionMethod");
            var response = _module.Execute(cmd);
            Assert.That(_ex.GetType()==typeof(MethodAccessException));
        }



        [Test]
        [Category("Execution")]
        public void CommandNotFound1Test()
        {
            var cmd = CommandBase.Create("Null");
            var response=_module.Execute(cmd);
            Assert.That(response.Contains("ERR_NOTFOUND"));
        }
        [Test]
        [Category("Execution")]
        public void CommandNotFound2Test()
        {            
            var response = _module.Execute("Null");
            Assert.That(response.Contains("ERR_NOTFOUND"));
        }
        [Test]
        [Category("Execution")]
        public void CommandNotFound3Test()
        {
            var cmd = CommandBase.Create("Null");
            _module.Execute(cmd.Serialize(), out string response);
            Assert.That(response.Contains("ERR_NOTFOUND"));
        }

        [Test]
        [Category("Execution")]
        public void ExecuteInICommandTest()
        {
            var cmd = CommandBase.Create("ReturnFalse");
            var result = JsonSerializer.Deserialize<bool>(_module.Execute(cmd));
            Assert.That(result, Is.False);
        }
        [Test]
        [Category("Execution")]
        public void ExecuteInICommandRAWTest()
        {
            var cmd = CommandBase.Create("ReturnFalse");
            var result = (bool)_module.ExecuteRaw(cmd);
            Assert.That(result, Is.False);
        }
        [Test]
        [Category("Execution")]
        public void ExecuteInMethodAndParametersTest()
        {
            var result = JsonSerializer.Deserialize<bool>(_module.Execute("ReturnFalse"));
            Assert.That(result, Is.False);
        }
        [Test]
        [Category("Execution")]
        public void ExecuteInMethodAndParametersRAWTest()
        {
            var result = (bool)_module.ExecuteRaw("ReturnFalse");
            Assert.That(result, Is.False);
        }
        [Test]
        [Category("Execution")]
        public void ExecuteInCommandStringTest()
        {
            var cmd = CommandBase.Create("ReturnFalse");
            var cmdStr = cmd.Serialize();
            _module.Execute(cmdStr, out var response);
            var result = JsonSerializer.Deserialize<bool>(response);
            Assert.That(result, Is.False);
        }
        [Test]
        [Category("Execution")]
        public void ExecuteInCommandStringRAWTest()
        {
            var cmd = CommandBase.Create("ReturnFalse");
            var cmdStr = cmd.Serialize();
            _module.ExecuteRaw(cmdStr, out var result);
            Assert.That(result, Is.False);
        }
        [Test]
        [Category("Execution")]
        public void ExecuteInICommandAsyncTest()
        {
            var cmd = CommandBase.Create("AsyncTestMethod", 100);
            var cmd2 = CommandBase.Create("AsyncTestMethod", 50);
            var index = Task.WaitAny(_module.ExecuteAsync(cmd), _module.ExecuteAsync(cmd2));
            Assert.That(index == 1);
        }
        [Test]
        [Category("Execution")]
        public void ExecuteInICommandRAWAsyncTest()
        {
            var cmd = CommandBase.Create("AsyncTestMethod", 100);
            var cmd2 = CommandBase.Create("AsyncTestMethod", 50);
            var index = Task.WaitAny(_module.ExecuteRawAsync(cmd), _module.ExecuteRawAsync(cmd2));
            Assert.That(index == 1);
        }
        [Test]
        [Category("Execution")]
        public void ExecuteInMethodAndParametersAsyncTest()
        {
            var index = Task.WaitAny(_module.ExecuteAsync("AsyncTestMethod", 100), _module.ExecuteAsync("AsyncTestMethod", 50));
            Assert.That(index == 1);
        }
        [Test]
        [Category("Execution")]
        public void ExecuteInMethodAndParametersRAWAsyncTest()
        {
            var index = Task.WaitAny(_module.ExecuteRawAsync("AsyncTestMethod", 100), _module.ExecuteRawAsync("AsyncTestMethod", 50));
            Assert.That(index == 1);
        }

        #endregion

        #region Plugin Test
        [Test()]
        [Category("Plugin")]
        public void InjectINotifierTest()
        {
            _module.InjectPlugin(new EmptyNotifier());
            
            Assert.That(_module.Notifier.Alias==nameof(EmptyNotifier));
        }
        [Test()]
        [Category("Plugin")]
        public void InjectIRecorderTest()
        {
            _module.InjectPlugin(new EmptyRecorder());
            Assert.That(_module.Recorder.Alias == nameof(EmptyRecorder));
        }
        [Test()]
        [Category("Plugin")]
        public void InjectITransceiverTest()
        {
            _module.InjectPlugin(new EmptyTransceiver());
            Assert.That(_module.Transceiver.Alias == nameof(EmptyTransceiver));
        }

        #endregion

    }
}