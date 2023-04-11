using System.Runtime.CompilerServices;

namespace MATSys.Hosting
{
    /// <summary>
    /// TestItemParameterAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TestItemParameterAttribute : Attribute
    {
        /// <summary>
        /// Type 
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// Name 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public TestItemParameterAttribute(Type type, [CallerMemberName] string name = "")
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
