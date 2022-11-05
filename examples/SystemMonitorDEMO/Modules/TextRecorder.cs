using MATSys;
using Microsoft.Extensions.Configuration;

namespace SystemMonitorDEMO.Modules
{
    internal class TextRecorder : IRecorder
    {
        private string fileName = "";
        private StreamWriter textWriter;
        public string Name => nameof(TextRecorder);

        public JObject Export()
        {
            throw new NotImplementedException();
        }

        public string Export(Formatting format = Formatting.Indented)
        {
            return "";
        }

        public void Load(IConfigurationSection section)
        {
        }

        public void Load(object configuration)
        {
        }

        public void StartService(CancellationToken token)
        {
            fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            textWriter = File.CreateText(fileName);

        }

        public void StopService()
        {
            textWriter.Flush();
            textWriter.Close();
        }

        public void Write(object data)
        {
            WriteAsync(data).Wait();
        }

        public async Task WriteAsync(object data)
        {
            await textWriter.WriteLineAsync(data.ToString());
        }
    }
}
