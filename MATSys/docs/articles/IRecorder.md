# IRecorder

namespace: <ins><b>MATSys.Plugins</b></ins> 

<pre><code>
    public interface IRecorder : IPlugin
    {
    
        // Write data to the instance        
        void Write(object data);

        
        // Write data to the instance asynchronuously        
        Task WriteAsync(object data);
    }
</code></pre>