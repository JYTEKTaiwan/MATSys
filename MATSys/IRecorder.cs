/* Unmerged change from project 'MATSys (netstandard2.0)'
Before:
namespace MATSys.Modules
After:
using MATSys;
using MATSys;
using MATSys.Modules
*/

namespace MATSys
{
    public interface IRecorder : IPlugin
    {
        /// <summary>
        /// Write data to the instance
        /// </summary>
        /// <param name="data">Data to be written</param>
        void Write(object data);

        /// <summary>
        /// Write data to the instance asynchronuously
        /// </summary>
        /// <param name="data">Data to be written</param>
        Task WriteAsync(object data);
    }

    public interface IRecorder<in T> : IRecorder
    {
        void Write(T data);

        Task WriteAsync(T data);
    }
}