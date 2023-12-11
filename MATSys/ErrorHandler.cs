using MATSys.Commands;


#if NET6_0_OR_GREATER

using System.Runtime.Loader;

#endif

namespace MATSys;

/// <summary>
/// Exception handle class for MATSys
/// </summary>
public class ExceptionHandler
{
    /// <summary>
    /// Prefix string when command is not found
    /// </summary>
    public const string cmd_notFound = "ERR_NOTFOUND";
    /// <summary>
    /// Prefix string when command reports error during execution
    /// </summary>
    public const string cmd_execError = "ERR_EXEC";
    /// <summary>
    /// Prefix string when command reports error during serialize/deserialize process
    /// </summary>
    public const string cmd_serDesError = "ERR_SerDes";

    /// <summary>
    /// Pring the error message
    /// </summary>
    /// <param name="prefix">Prefix string</param>
    /// <param name="ex">Excption instance</param>
    /// <param name="commandString">ICommand in string format</param>
    /// <returns></returns>
    public static string PrintMessage(string prefix, Exception ex, string commandString)
    {
        return $"[{prefix}] {ex.Message} - {commandString}";
    }
    /// <summary>
    /// Pring the error message
    /// </summary>
    /// <param name="prefix">Prefix string</param>
    /// <param name="ex">Excption instance</param>
    /// <param name="command">ICommand instance</param>
    /// <returns></returns>
    public static string PrintMessage(string prefix, Exception ex, ICommand command)
    {
        return $"[{prefix}] {ex.Message} - {command.Serialize()}";
    }

}

