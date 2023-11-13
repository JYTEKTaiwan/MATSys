# Built-in Plugins

namespace: <ins><b>MATSys.Plugins</b></ins> 

## **Transceiver**
### <u>EmptyTransceiver</u>
Default transceiver instance, doing nothing
### <u>NetMQTransceiver</u>
REQ/REP is implemented using NetMQRouterSocket and NetMQDealerSocket.
<pre><code>
    //Optional parameter
    public class NetMQTransceiverConfiguration : IMATSysConfiguration
    {
        public string Type { get; set; } = "netmq";
        public bool EnableLogging { get; set; } = false;
        public string LocalIP { get; set; } = "";
        public int Port { get; set; } = -1;
    }
</code></pre>

## **Notifier**
### <u>EmptyNotifgier</u>
Default notifier instance, doing nothing
### <u>NetMQNotifier</u>
Pub/Sub is implemented using NetMQPublisherSocket and NetMQSubscriberSocket.
<pre><code>
    //Optional parameters
    public class NetMQNotifierConfiguration : IMATSysConfiguration
    {
        public string Type { get; set; } = "netmq";
        public bool EnableLogging { get; set; } = false;
        public string Address { get; set; } = "127.0.0.1:5000";
        public string Protocal { get; set; } = "tcp";
        public string Topic { get; set; } = "";
        public bool DisableEvent { get; set; } = true;

        public static NetMQNotifierConfiguration Default => new NetMQNotifierConfiguration();
    }
</code></pre>

## **Recorder**
### <u>EmptyRecorder</u>
Default recorder instance, doing nothing
### <u>CSVRecorder</u>
Write object data into CSV file. User should define the dataset in class. CSVRecorder will parse all the properties and writes to the file separately.
<pre><code>
    //Optional paramters
    public class CSVRecorderConfiguration : IMATSysConfiguration
    {
        public string Type { get; set; } = "csv";
        public bool EnableLogging { get; set; } = false;
        public bool WaitForComplete { get; set; } = true;
        public int QueueSize { get; set; } = 2000;
        public int WaitForCompleteTimeout { get; set; } = 5000;
        public static CSVRecorderConfiguration Default = new CSVRecorderConfiguration();

        public BoundedChannelFullMode BoundedChannelFullMode { get; set; } = BoundedChannelFullMode.DropOldest;
    }
</code></pre>