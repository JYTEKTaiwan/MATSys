using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Modules
{
    public interface IConfigurationResolver
    {
        void Load(object configuration);
    }
}
