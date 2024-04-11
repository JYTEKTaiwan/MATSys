using MATSys;
namespace MockModulesLibrary
{
    public class MockModule1 : ModuleBase
    {
        internal class Config
        {
            public int Test { get; set; }
        }
        public MockModule1 ()
        {
            base.Configuration = new Config();
        }
    }
}
