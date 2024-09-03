namespace MATSys.Commands
{
    /// <summary>
    /// Interface for Command
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Name of the Command 
        /// </summary>
        /// <value></value>
        string MethodName { get; set; }


        /// <summary>
        /// Get the list of the parameter objects in command
        /// </summary>
        /// <returns></returns>
        object[]? GetParameters();

        /// <summary>
        /// Serialize the command object. Format is MethodName=Parameter1,Parameter2,...,
        /// Each Parameter is in json format
        /// </summary>
        /// <returns></returns>
        string Serialize();

    }
}