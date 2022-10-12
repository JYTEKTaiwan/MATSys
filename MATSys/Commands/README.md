README# Command Pattern Design

namespace: <ins><b>MATSys.Commands</b></ins> 

MATSys.Commands packages several features that helps developer to invoke method in Module using string format.
- Command class - information of the method access
- MethodInvoker class - delegation of the Module instance
- MATSysCommandAttribute class - used to tag the method user wants to aceess

<b>Module</b>, <b>Command</b> and <b>MethodInvoker</b> are 3 independent instances that handle and remote access behavior. 

Command is created and sent to the Module, and MethodInvoker then uses this information to access(delegate) the method in the Module.To be noted that only methods with <b>MATSysCommandAttribute</b> can be accessed through MethodInvoker.

Let's start with the Command first

## Command
MATSys provides 8 different Command classes that keep the information
<pre><code>
    Command
    Command&lt;T1>
    Command&lt;T1,T2>
    Command&lt;T1,T2,T3>
    Command&lt;T1,T2,T3,T4>
    Command&lt;T1,T2,T3,T4,T5>
    Command&lt;T1,T2,T3,T4,T5,T6>
    Command&lt;T1,T2,T3,T4,T5,T6,T7>

    // T1~T7 represnet the parameter in the method
</code></pre>
Command instance contains the method name and all the parameters information for later usage. Every command instance can be created and kept using abstract <b>CommanbdBase</b>. 

To be noted that operators like ref, out are not supported.

## MethodInvoker
MATSys.Commands use MethodInvoker instance internally to access the method using delegation. In most case user don't need to use or modify it.
Because MethodInvoker use delegate to access methods, it provides a high performance access compared to reflection.

## MATSysCommandAttribute
In the following example, this is a normal method without <b>MATSysCommandAttribute</b> assigned. This means user cannot access the method by MethodInvoker but only use direct method call.
<pre><code>
    public string Hello(string input)
    {
        //return something;
    }
</code></pre>

Once the <b>MATSysCommandAttribute</b> is assigned, this method can now be access through MethodInvoker using Command instance.

<pre><code>
    [MATSysCommand("Test",typeof(Command&lt;string>)]
    public string Hello(string input)
    {
        //return something;
    }
    //this method will be access with name "Test" and type Command&lt;string> assigned by attribute
</code></pre>

<b>MATSysCommandAttribute</b> has two parameters, first is the alias name of the method(use default if empty). The second is the type of the Command.
User can leave the parameters empty, system will automatically choose the default value.

<pre><code>
    [MATSysCommand]
    public string Hello(string input)
    {
        //return something;
    }
    //this method will be access with name "Hello" and type Command&lt;string> from the parameters definition
</code></pre>


