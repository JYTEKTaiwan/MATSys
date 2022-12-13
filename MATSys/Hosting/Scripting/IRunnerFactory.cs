using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Hosting.Scripting
{
    /// <summary>
    /// Interface of factory that used to create runner
    /// </summary>
    internal interface IRunnerFactory
    {
        /// <summary>
        /// Create Runner instance
        /// </summary>
        /// <returns><see cref="IRunner"/> instance</returns>
        IRunner CreateRunner();

    }
}
