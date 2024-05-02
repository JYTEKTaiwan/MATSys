using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MATSys.Commands;

namespace MATSys
{
    public sealed class ModuleContext
    {
        public string TypeName { get; }
        public IEnumerable<string> SupportedMethods { get; }
        private ModuleContext(Type t)
        {
            SupportedMethods = t.GetMethods()
                .Where(x => x.GetCustomAttributes(typeof(MATSysCommandAttribute), true).Count() > 0)
                .Select(m=>m.ToString());
            TypeName = t.Name;
        }

        public static ModuleContext Parse(Type modType)
        {
            if (typeof(IModule).IsAssignableFrom(modType))
            {
                return new ModuleContext(modType);
            }
            else
            {
                throw new NotSupportedException($"Type named {modType} is not supported");
            }

        }
        public static ModuleContext Parse<T>() where T:IModule
        {
            return Parse(typeof(T));
        }

    }

}
