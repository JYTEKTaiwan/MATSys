namespace MATSys.Hosting
{
    public class TestPackageContext
    {
        public string AssemblyPath { get; set; }
        public string Alias { get; set; }
        public string Type { get; set; }
        public TestPackageContext(string assemblyPath, string alias, string type)
        {
            AssemblyPath = assemblyPath;
            Alias = alias;
            Type = type;
        }
    }
}
