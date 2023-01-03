using MATSys.Commands;
using System.Diagnostics;

namespace UT_MATSys;

public class UT_Command
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ResultIsNullOrEmpty()
    {
        var cmd = CommandBase.Create("Test", 1, 2.0);
        var str = cmd.ConvertResultToString(null);
        Assert.IsTrue(string.IsNullOrEmpty(str));
    }

    [Test]
    public void SerDesTest()
    {
        var cmd = CommandBase.Create("Test", 1, 2.0);
        var str = cmd.Serialize();
        var att = new MATSysCommandAttribute("Test", typeof(Command<int, double>));
        var res = CommandBase.Deserialize(str, att.CommandType) as Command<int, double>;
        Assert.IsTrue(res!.Item1 == 1 && res.Item2 == 2.0);
    }

    [Test]
    public void SerDesTestWithBoolean()
    {
        var cmd = CommandBase.Create("Test", true);
        var str = cmd.Serialize();
        var att = new MATSysCommandAttribute("Test", typeof(Command<Boolean>));
        var res = CommandBase.Deserialize(str, att.CommandType) as Command<Boolean>;
        Assert.IsTrue(res!.Item1 == true);
    }
    [Test]
    public void SerDesTestWithCustomObject()
    {
        var cmd = CommandBase.Create("Test", 1, new Test(20));
        var str = cmd.Serialize();
        var att = new MATSysCommandAttribute("Test", typeof(Command<int, Test>));
        var res = CommandBase.Deserialize(str, att.CommandType) as Command<int, Test>;
        Assert.IsTrue(res!.Item1 == 1 && res.Item2.A == 20);
    }
    [Test]
    public void SerDesTestWithWrongType()
    {

        Assert.Catch<System.Text.Json.JsonException>(() =>
        {
            var cmd = CommandBase.Create("Test", "");
            var str = cmd.Serialize();
            var att = new MATSysCommandAttribute("Test", typeof(Command<int>));
            var res = CommandBase.Deserialize(str, att.CommandType) as Command<int>;

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
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }
}

public class UT_Command_Performance
{
    [Test]
    public void SevenArgumentsSerialization()
    {
        var cmd = CommandBase.Create("Test", 1, 1.0, (decimal)1, true, DateTime.Now, new object(), "");
        var str = cmd.Serialize();
        var cnt = 10000;
        Stopwatch sw = new Stopwatch();
        sw.Restart();
        for (int i = 0; i < cnt; i++)
        {
            str = cmd.Serialize();
        }
        sw.Stop();
        TestContext.Out.WriteLine($"Len={str.Length} bytes, {sw.Elapsed.TotalSeconds / cnt}ms");
        Assert.Pass();
    }
    [Test]
    public void SevenArgumentsDeserialization()
    {
        var cmd = CommandBase.Create("Test", 1, 1.0, (decimal)1, true, DateTime.Now, new object(), "");
        var str = cmd.Serialize();
        var t = cmd.GetType();
        var obj = CommandBase.Deserialize(str, t);
        var cnt = 10000;
        Stopwatch sw = new Stopwatch();
        sw.Restart();
        for (int i = 0; i < cnt; i++)
        {
            obj = CommandBase.Deserialize(str, t);
        }
        sw.Stop();
        TestContext.Out.WriteLine($"Len={str.Length} bytes, {sw.Elapsed.TotalSeconds / cnt}ms");
        Assert.Pass();
    }

    [Test]
    public void SevenArgumentsValueTupleDeserialization()
    {
        var cmd = ValueTuple.Create(1, 1.0, (decimal)1, true, DateTime.Now, new object(), "");
        var str = Newtonsoft.Json.JsonConvert.SerializeObject(cmd);
        var t = typeof(ValueTuple<int, double, decimal, bool, DateTime, object, string>);
        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(str, t);
        var cnt = 10000;
        Stopwatch sw = new Stopwatch();
        sw.Restart();
        for (int i = 0; i < cnt; i++)
        {
            obj = Newtonsoft.Json.JsonConvert.DeserializeObject(str, t);
        }
        sw.Stop();
        TestContext.Out.WriteLine($"Len={str.Length} bytes, {sw.Elapsed.TotalSeconds / cnt}ms");
        Assert.Pass();
    }


}
