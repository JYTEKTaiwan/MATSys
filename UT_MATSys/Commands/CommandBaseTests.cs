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
    public class CommandBaseTests
    {
        [Test]
        public void ResultIsNullOrEmpty()
        {
            var cmd = CommandBase.Create("Test", 1, 2.0);
            var str = cmd.ConvertResultToString(null);
            Assert.That(string.IsNullOrEmpty(str));
        }

        [Test]
        public void SerDesTest()
        {            
            var cmd = CommandBase.Create("Test", 1, 2.0);
            var str = cmd.Serialize();
            var res = CommandBase.Deserialize(str, typeof(Command<int, double>)) as Command<int, double>;
            Assert.That(res!.Item1 == 1 && res.Item2 == 2.0);
        }

        [Test]
        public void SerDesTestWithBoolean()
        {
            var cmd = CommandBase.Create("Test", true);
            var str = cmd.Serialize();
            var parameters = CommandBase.Deserialize(str, typeof(Command<bool>)).GetParameters();
            Assert.That((bool)parameters[0] == true);
        }
        [Test]
        public void SerDesTestWithCustomObject()
        {
            var cmd = CommandBase.Create("Test", 1, new Test(20));
            var str = cmd.Serialize();
            var res = CommandBase.Deserialize(str, typeof(Command<int, Test>)) as Command<int, Test>;
            Assert.That(res!.Item1 == 1 && res.Item2.A == 20);
        }
        [Test]
        public void SerDesTestWithWrongType()
        {
            Assert.Catch<System.Text.Json.JsonException>(() =>
            {
                var cmd = CommandBase.Create("Test", "");
                var str = cmd.Serialize();
                var res = CommandBase.Deserialize(str, typeof(Command<int>)) as Command<int>;
            });
        }
        private class Test
        {
            public int A { get; }

            public Test(int a)
            {
                A = a;
            }
        }

        [Test]
        public void CommandCreate([Range(0, 7, 1)] int i)
        {
            try
            {
                switch (i)
                {
                    case 0:
                        CommandBase.Create("Test");
                        break;

                    case 1:
                        CommandBase.Create("Test", i);
                        break;

                    case 2:
                        CommandBase.Create("Test", i, i);
                        break;

                    case 3:
                        CommandBase.Create("Test", i, i, i);
                        break;

                    case 4:
                        CommandBase.Create("Test", i, i, i, i);
                        break;

                    case 5:
                        CommandBase.Create("Test", i, i, i, i, i);
                        break;

                    case 6:
                        CommandBase.Create("Test", i, i, i, i, i, i);
                        break;

                    case 7:
                        CommandBase.Create("Test", i, i, i, i, i, i, i);
                        break;
                }
                Assert.That(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}