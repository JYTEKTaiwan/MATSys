using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys
{
    internal class ExceptionHandler
    {
        public static string PrintMessage(string prefix, Exception ex, string commandString)
        {
            return $"[{prefix}] {ex.Message} - {commandString}";
        }
    }
}
