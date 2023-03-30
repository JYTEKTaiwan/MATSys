using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Hosting
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TestItemParameterAttribute:Attribute
    {
        public Type Type { get; }
        public string Name { get; }
        public TestItemParameterAttribute(Type type,[CallerMemberName] string name = "")
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
