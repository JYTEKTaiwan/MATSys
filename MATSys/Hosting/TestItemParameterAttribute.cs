using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Hosting
{
    internal class TestItemParameterAttribute:Attribute
    {
        public Type Type { get; }
        public TestItemParameterAttribute(Type type)
        {
            Type = type;
        }
    }
}
