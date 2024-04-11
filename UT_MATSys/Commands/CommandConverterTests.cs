using NUnit.Framework;
using MATSys.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Commands.Tests
{
    [TestFixture()]
    public class CommandConverterTests
    {
        [MATSysCommandContract("CommandClass")]
        internal class CommandTest
        {
            [MATSysCommandOrder(1)]
            public bool Property1 { get; set; }

            [MATSysCommandOrder(2)]
            public int Property2 { get; set; }
        }

        [MATSysCommandContract("CommandRecord")]
        internal record CommandRecordTest(bool first, int second);
        [Test()]
        public void ConvertClassTest()
        {
            var cmdPOCO=new CommandTest();
            cmdPOCO.Property1 = true;
            cmdPOCO.Property2 = 500;
            var cmd= CommandConverter.Convert(cmdPOCO);
            var parameters = cmd.GetParameters();
            Assert.That(cmd.MethodName == "CommandClass" && (bool)parameters[0] == true && (int)parameters[1] == 500);
        }

        [Test()]
        public void TryConvertClassTest()
        {
            var cmdPOCO = new CommandTest();
            cmdPOCO.Property1 = true;
            cmdPOCO.Property2 = 500;
            var pass = CommandConverter.TryConvert(cmdPOCO, out var cmd);
            var parameters = cmd.GetParameters();
            Assert.That(pass&&cmd.MethodName == "CommandClass" && (bool)parameters[0] == true && (int)parameters[1] == 500);
        }
        [Test()]

        public void ConvertRecordTest()
        {
            var cmdPOCO = new CommandRecordTest(true, 500);
            var cmd = CommandConverter.Convert(cmdPOCO);
            var parameters = cmd.GetParameters();
            Assert.That(cmd.MethodName == "CommandRecord" && (bool)parameters[0] == true && (int)parameters[1] == 500);
        }

        [Test()]
        public void TryConvertRecordTest()
        {
            var cmdPOCO = new CommandRecordTest(true, 500);
            var pass = CommandConverter.TryConvert(cmdPOCO, out var cmd);
            var parameters = cmd.GetParameters();
            Assert.That(pass && cmd.MethodName == "CommandRecord" && (bool)parameters[0] == true && (int)parameters[1] == 500);
        }

    }
}