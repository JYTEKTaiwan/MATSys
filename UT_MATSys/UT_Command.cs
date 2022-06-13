using MATSys.Commands;

namespace UT_MATSys
{
    public class UT_Command
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ResultIsNullOrEmpty()
        {
            var cmd = CommandBase.Create("Test",1,2.0);
            Assert.IsTrue(string.IsNullOrEmpty(cmd.ConvertResultToString("")));
        }
    }
}