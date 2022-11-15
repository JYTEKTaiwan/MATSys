using System.Text.Json.Serialization;

namespace MATSys
{
    /// <summary>
    /// Common class which store the test result information
    /// </summary>
    public class TestItemResult
    {
        public TestItemResult() { }

        /// <summary>
        /// DateTime information
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Type of the test result (Uses <see cref="TestResultType"/>)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TestResultType Result { get; set; }

        /// <summary>
        /// Bin number
        /// </summary>
        public int Bin { get; set; }

        /// <summary>
        /// Value for after the execution
        /// </summary>
        public object? Value { get; set; }
        /// <summary>
        /// Any other arguments or attributes
        /// </summary>
        public object? Attributes { get; set; }
        /// <summary>
        /// Create new instance of TestItemResult
        /// </summary>
        /// <param name="result">Type:<see cref="TestResultType">TestResultType</see></param>
        /// <param name="bin">Bin Number</param>
        /// <param name="value">Raw value</param>
        /// <param name="attributes">Custom attributes</param>
        /// <returns>Instance of TestItemResult</returns>
        public static TestItemResult Create(TestResultType result = TestResultType.Skip, int bin = -1, object? value = null, object? attributes = null)
        {
            return new TestItemResult()
            {
                Result=result,
                Bin=bin,
                Value=value,
                Attributes=attributes
                };
            
        }
    }

    /// <summary>
    /// Type of test result
    /// </summary>
    public enum TestResultType
    {
        /// <summary>
        /// Pass (1)
        /// </summary>
        Pass = 1,
        /// <summary>
        /// Fail (-1)
        /// </summary>
        Fail = -1,
        /// <summary>
        /// Skip (0)
        /// </summary>
        Skip = 0

    }
}
