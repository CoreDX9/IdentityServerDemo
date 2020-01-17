using Grpc.Net.Client;
using IdentityServer.Grpc.Services;
using System;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var url = string.Empty;
#if DEBUG
            url = "https://localhost:5001";
#else
            url = "https://localhost";
#endif

            var channel = GrpcChannel.ForAddress(url);
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                new HelloRequest { Name = "CoreDX" });
            Console.WriteLine("Greeter 服务返回数据: " + reply.Message);
            Console.ReadKey();
        }
    }
}
