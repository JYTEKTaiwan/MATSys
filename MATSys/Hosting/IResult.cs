using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MATSys.Hosting
{
    public interface IResult
    {
        DateTime TimeStamp { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        ResultStatus Status { get; set; }
    }
    public class TestResult:IResult
    {
        
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResultStatus Status { get; set; } = ResultStatus.Skip;

    }
    public enum ResultStatus
    {
        Pass,
        Fail,
        Skip
    }

}
