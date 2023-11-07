# ITransceiver

namespace: <ins><b>MATSys.Plugins</b></ins> 

<pre><code>
    public interface ITransceiver : IPlugin
    {
    
        // Indicate whether new request is happened        
        delegate string RequestFiredEvent(object sender, string commandObjectInJson);
        
        // Event for the new coming request
        event RequestFiredEvent? OnNewRequest;

    }
</code></pre>