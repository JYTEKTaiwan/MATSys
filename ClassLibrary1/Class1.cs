using System.Globalization;

namespace ClassLibrary1
{
    public class Class1
    {
        public void DoCSV()
        {
            using (var sw = new StreamWriter(@$".\{Random.Shared.Next().GetHashCode()}.txt"))
            {
                using (var writer = new CsvHelper.CsvWriter(sw, System.Globalization.CultureInfo.InvariantCulture))
                {
                    writer.WriteRecord(new Data());
                    writer.NextRecord();
                }

            }
        }
    }

    public class Data
    {
        public string Content { get; }
        public Data() { Content = DateTime.Now.ToString(); }

    }

}