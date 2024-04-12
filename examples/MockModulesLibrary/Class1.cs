using MATSys;
namespace MockModulesLibrary
{
    public class MockModule1 : ModuleBase
    {
        public override object Configuration { get; set; } = new object();
        internal class Config
        {
            public int Test { get; set; }
        }
        public MockModule1 ()
        {
        }
    }
}
