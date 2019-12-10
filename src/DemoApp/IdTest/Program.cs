using IdentityServer;
using Microsoft.Extensions.Hosting;
using System;

namespace IdTest
{
    //将编译出来的dll和exe复制到IdentityServer的发布目录，然后运行。可以将网站项目的主模块作为类库使用。可以间接把网站嵌入winform或wpf中而不必把网站作为单独的进程用Process类启动并管理。
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var host = IdentityServer.Program.CreateHostBuilderP(args).Build();
            SeedData.EnsureSeedData(host.Services);//初始化数据库
            await host.RunAsync();
        }
    }
}
