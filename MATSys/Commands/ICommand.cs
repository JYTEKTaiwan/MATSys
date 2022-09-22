using Newtonsoft.Json;

namespace MATSys.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Name of the Command 
        /// </summary>
        /// <value></value>
        string MethodName { get; set; }

        /// <summary>
        /// Convert the object returned from the execution into string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string? ConvertResultToString(object? obj);

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