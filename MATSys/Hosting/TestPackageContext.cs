namespace MATSys.Hosting
{
    /// <summary>
    /// Basic context for test package
    /// </summary>
    public class TestPackageContext
    {
        /// <summary>
        /// Path of the assembly
        /// </summary>
        public string AssemblyPath { get; set; } = "";
        /// <summary>
        /// Alias
        /// </summary>
        public string Alias { get; set; } = "";
        /// <summary>
        /// Type of the test package
        /// </summary>
        public string Type { get; set; } = "";
        /// <summary>
        /// ctor
        /// </summary>
        public TestPackageContext() { }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="alias"></param>
        /// <param name="type"></param>
        public TestPackageContext(string assemblyPath, string alias, string type)
        {
            AssemblyPath = assemblyPath;
            Alias = alias;
            Type = type;
        }
    }
}
