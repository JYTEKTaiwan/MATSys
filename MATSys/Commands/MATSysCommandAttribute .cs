namespace MATSys.Commands
{
    /// <summary>
    /// Attribute that represent the method information for MATSys to use
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MATSysCommandAttribute : Attribute
    {
        /// <summary>
        /// Name of the ICommand instance
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Type of the ICommand instance
        /// </summary>
        public Type? CommandType { get; set; }


#if NET6_0_OR_GREATER || NETSTANDARD2_0
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Name">Method name (if not assigned, use original method name instead)</param>
        /// <param name="Type">Type for the Command (if not assigned, user should call ConfigureCommandType to assign manually)</param>
        public MATSysCommandAttribute([CallerMemberName] string Name = "", Type? Type = null)
        {
            Alias = Name;
            CommandType = Type;
        }
#elif NET35
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Name">Method name (if not assigned, use original method name instead)</param>
        /// <param name="Type">Type for the Command (if not assigned, user should call ConfigureCommandType to assign manually)</param>
        public MATSysCommandAttribute(string Name, Type? Type = null)
        {
            Alias = Name;
            CommandType = Type;
        }
#endif
    }
}
