using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MATSys.Hosting
{
    public interface ITestPackage
    {
        string Alias { get; set; }
        IServiceProvider Provider { get; }

        JsonNode Execute(string testItemName, JsonNode parameter);
        void InjectServiceProvider(IServiceProvider serviceProvider);

    }
}
