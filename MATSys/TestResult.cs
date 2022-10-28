using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys
{
    public struct TestItemResult
    {
        public DateTime Timestamp { get; } = DateTime.Now;
        public Classification Result { get; }
        public int BinNumber { get; }
        public object Value { get; }
        public object Attributes { get; }

        public TestItemResult(Classification result, int bin, object value, object attributes, DateTime dt)
        {
            if (dt != null)
            {
                Timestamp = dt;
            }
            BinNumber = bin;
            Result = result;
            Value = value;
            Attributes = attributes;
        }
    }

    public enum Classification
    {
        Pass,
        Fail,
        Skip

    }
}
