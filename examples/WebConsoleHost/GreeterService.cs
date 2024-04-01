using MATSys.Commands;
using MATSys.Hosting;
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

    public async Task<HelloReply> ClientStreamCall(IAsyncEnumerable<HelloRequest> request, CallContext context = default)
    {
        return await Task.Run(async () =>
        {
            string txt = "";
            var ch = request.GetAsyncEnumerator();
            while (await ch.MoveNextAsync())
            {
                var item = ch.Current;
                txt += item.Name + ";";
            }
            return new HelloReply() { Message = txt };
        });

    }

    public async Task<HelloReply> SayHelloAsync(HelloRequest request, CallContext context)
    {

        return await Task.Run(async () =>
        {
            var a = _provider.GetModule("Dev1");
            var cmd = CommandConverter.Convert(request);
            var reply = new HelloReply() { Message = await a.ExecuteAsync(cmd) };
            return reply;
        });
    }

    public async IAsyncEnumerable<HelloReply> ServerStreamCall(HelloRequest request, CallContext context = default)
    {
        for (var i = 0; i < int.Parse(request.Name); i++)
        {
            yield return new HelloReply() { Message = i.ToString() };
        }
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

    [OperationContract]
    IAsyncEnumerable<HelloReply> ServerStreamCall(HelloRequest request,
        CallContext context = default);

    [OperationContract]
    Task<HelloReply> ClientStreamCall(IAsyncEnumerable<HelloRequest> request,
    CallContext context = default);



}
