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
        /// <param name="section">Configuration section in json file</param>
        /// <param name="enabled">if script mode is enable or not</param>
        /// <returns><see cref="IRunner"/> instance</returns>
        IRunner CreateRunner(IConfigurationSection section,bool enabled);
        /// <summary>
        /// Create Runner instance
        /// </summary>
        /// <param name="type">Type of the runner</param>
        /// <param name="section">Configuration section in json file</param>
        /// <returns></returns>
        IRunner CreateRunner(Type type,IConfigurationSection section);

        void Load(IConfigurationSection section);


    }
}
