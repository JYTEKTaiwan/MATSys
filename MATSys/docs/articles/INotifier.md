# INotifier

namespace: <ins><b>MATSys</b></ins> 

<pre><code>
    public interface INotifier : IPlugin
    {
 
        // Indicate that new data is ready to notify     
        delegate void NotifyEvent(string dataInJson);
        
        // Event when new data is ready to notify        
        event NotifyEvent? OnNotify;
       
        //Publish the data
        void Publish(object data);

        // Get the latest data stored in the buffer. Return null if timeout
        object? GetData(int timeoutInMilliseconds = 1000);

    }
</code></pre>