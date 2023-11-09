using MATSys.Commands;
using ProtoBuf;
using ProtoBuf.Grpc;
using System.ServiceModel;

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

    public async Task<HelloReply> SayHelloAsync(HelloRequest request, CallContext context)
    {
        return await Task.Run(async () =>
        {
            var a = _provider.GetRequiredService<ModuleActivator>().Create("Dev1");
            var cmd = CommandConverter.Convert(request);
            var reply = new HelloReply() { Message = await a.ExecuteAsync(cmd) };
            return reply;
        });
        // return Task.FromResult(new HelloReply(){Message="A"});
    }

}

[ProtoContract]
public class HelloReply
{
    [ProtoMember(1, Name = @"message")]
    public string Message { get; set; } = "";
}

[ProtoContract]
[MATSysCommandContract("Hello")]
public class HelloRequest
{
    [ProtoMember(1, Name = @"name")]
    [MATSysCommandOrder(0)]
    public string Name { get; set; } = "";

}

[ServiceContract(Name = @"greet.Greeter")]
public partial interface IGreeterService
{
    [OperationContract]
    Task<HelloReply> SayHelloAsync(HelloRequest request,
        CallContext context = default);
}
