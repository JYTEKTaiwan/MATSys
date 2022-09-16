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
        public string Alias { get; }

        /// <summary>
        /// Type of the ICommand instance
        /// </summary>
        public Type? CommandType { get; set; }
        
        public MethodInvoker? Invoker { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="t"></param>
        public MATSysCommandAttribute([CallerMemberName]string Name="", Type? Type=null)
        {
            Alias = Name;
            CommandType = Type;
        }

    }
}