namespace MATSys
{
    /// <summary>
    /// Common class which store the test result information
    /// </summary>
    public class TestItemResult
    {
        /// <summary>
        /// DateTime information
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.Now;

        /// <summary>
        /// Type of the test result (Uses <see cref="TestResultType"/>)
        /// </summary>
        public TestResultType Result { get; }

        /// <summary>
        /// Bin number
        /// </summary>
        public int BinNumber { get; }

        /// <summary>
        /// Value for after the execution
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// Any other arguments or attributes
        /// </summary>
        public object Attributes { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="bin">Bin number</param>
        /// <param name="value">Value</param>
        /// <param name="attributes">Attributes</param>
        public TestItemResult(TestResultType result, int bin, object value, object attributes)
        {
            BinNumber = bin;
            Result = result;
            Value = value;
            Attributes = attributes;
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
