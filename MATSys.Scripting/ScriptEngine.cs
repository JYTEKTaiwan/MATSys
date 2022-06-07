using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.RegularExpressions;

namespace MATSys.Scripting
{
    public class ScriptEngine
    {
        private ScriptOptions scriptOpt = ScriptOptions.Default;
        private Script? cs;        
        private const string pattern = @"(?:using\s+)(?<assembly>[a-zA-Z0-9.]+)(?:;)";
        private CancellationTokenSource? cts;
        public void Load(string fileName)
        {
            string code = File.ReadAllText(Path.GetFullPath(fileName));
            RegexOptions options = RegexOptions.IgnoreCase;
            List<string> assemblies = new List<string>();
            foreach (Match m in Regex.Matches(code, pattern, options))
            {
                Console.WriteLine("'{0}' found at index {1}.", m.Groups["assembly"].Value, m.Index);
                assemblies.Add(m.Groups["assembly"].Value);
            }
            cs = CSharpScript.Create(code, ScriptOptions.Default.AddReferences(assemblies.ToArray()));
            cs.Compile();
        }
        public ScriptState Run(int timeoutMilliSecond)
        {
            cts = new CancellationTokenSource();
            var res = RunAsync();
            res.Wait(timeoutMilliSecond, cts.Token);
            return res.Result;
        }
        public async Task<ScriptState> RunAsync()
        {
            return await cs!.RunAsync(cts!.Token);
        }
        public void Stop()
        {
            cts!.Cancel();
        }
    }

    public static class ExtendStateOption
    {
        public static object GetVariable<T>(this ScriptState state, string name)
        {
            return (T)state.Variables.First(x => x.Name == name).Value;
        }
    }
}