using System.Diagnostics.Contracts;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class TestResult
{
    [XmlAttribute]public DateTime TimeStamp { get; set; }
    [XmlAttribute] public TestResultType Result { get; set; }
    [XmlAttribute] public int IterationCount { get; set; }
    [XmlAttribute] public string Value { get; set; }

    public string Message { get; set; }
    public static TestResult Create(TestResultType result, string value, int Iteration = 0)
    {
        return new TestResult()
        {
            TimeStamp = DateTime.Now,
            Value = value==null?"":value.ToString(),
            Result = result,
            IterationCount = Iteration
        };
    }

    public XmlSchema? GetSchema() => null;


    public override string ToString()
    {
        return $"[{TimeStamp:yyyy/MM/dd HH:mm:ss.fff}] {Result}:{Value} #{IterationCount}";
    }

}
