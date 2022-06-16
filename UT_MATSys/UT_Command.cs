using MATSys.Commands;

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
        Assert.IsTrue(string.IsNullOrEmpty(cmd.ConvertResultToString("")));
    }

    [Test]
    public void SerDesTest()
    {
        var cmd = CommandBase.Create("Test", 1, 2.0);
        var str = Newtonsoft.Json.JsonConvert.SerializeObject(cmd);
        var res = Newtonsoft.Json.JsonConvert.DeserializeObject(str, cmd.GetType()) as Command<int, double>;
        Assert.IsTrue(res!.Parameter.Item1 == 1 && res.Parameter.Item2 == 2.0);
    }

    [Test]
    public void SerDesTestWithCustomObject()
    {
        var cmd = CommandBase.Create("Test", 1, new Test(20));
        var str = Newtonsoft.Json.JsonConvert.SerializeObject(cmd);
        var res = Newtonsoft.Json.JsonConvert.DeserializeObject(str, cmd.GetType()) as Command<int, Test>;
        Assert.IsTrue(res!.Parameter.Item1 == 1 && res.Parameter.Item2.A == 20);
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


