using System.Text.Json.Nodes;

namespace MATSys.Hosting
{
    public interface ITestPackage : IDisposable
    {
        string Alias { get; set; }
        IServiceProvider Provider { get; }

        JsonNode Execute(string testItemName, JsonNode parameter);
        void InjectServiceProvider(IServiceProvider serviceProvider);

    }
}
