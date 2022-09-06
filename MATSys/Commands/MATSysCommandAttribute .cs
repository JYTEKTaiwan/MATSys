using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MATSys.Commands
{
    /// <summary>
    /// Attribute that represent the method information for MATSys to use
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MATSysCommandAttribute: Attribute
    {
        /// <summary>
        /// Name of the ICommand instance
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Type of the ICommand instance
        /// </summary>
        public Type? CommandType { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="t"></param>
        public MATSysCommandAttribute([CallerMemberName]string name="", Type? t=null)
        {
            Name = name;
            CommandType = t;
        }

    }
}