using System.Diagnostics;
using System.Runtime.Serialization;
using System.ServiceModel;
using Grpc.Core;
using MATSys;
using MATSys.Commands;
using MATSys.Factories;
using Microsoft.VisualBasic;
using ProtoBuf;
using ProtoBuf.Grpc;

namespace WebConsoleHost;

public class GreeterService : IGreeterService
{
    private readonly ILogger<GreeterService> _logger;
    private readonly IServiceProvider _provider;
    public GreeterService(ILogger<GreeterService> logger, IServiceProvider provider)
    {
        _logger = logger;
        _provider = provider;

    }
  
    public Task<HelloReply> SayHelloAsync(HelloRequest request, CallContext context)
    {
        return Task.Run(() =>
        {
            var a = _provider.GetRequiredService<ModuleActivator>().Create("Dev1");
            var reply=new HelloReply() { Message = a.Execute(request.ConvertToCommand()) };        
            return reply;
        });
        // return Task.FromResult(new HelloReply(){Message="A"});
    }

}

[ProtoContract]
public class HelloReply
{
    [ProtoMember(1, Name = @"message")]
    public string Message { get; set; }

    
}

[ProtoContract]
public class HelloRequest:ICommandConvertable
{
    [ProtoMember(1, Name = @"name")]
    public string Name { get; set; }

    public ICommand ConvertToCommand()
    {
        return CommandBase.Create("Hello",Name);
    }
}

[ServiceContract(Name = @"greet.Greeter")]
public partial interface IGreeterService
{
    [OperationContract]
    Task<HelloReply> SayHelloAsync(HelloRequest request,
        CallContext context = default);
}
