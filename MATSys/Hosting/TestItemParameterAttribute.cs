using System.Runtime.CompilerServices;

namespace MATSys.Hosting
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TestItemParameterAttribute : Attribute
    {
        public Type Type { get; }
        public string Name { get; }
        public TestItemParameterAttribute(Type type, [CallerMemberName] string name = "")
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
