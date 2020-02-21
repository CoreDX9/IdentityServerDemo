using BlazorApp.Client.Extensions;
using Microsoft.AspNetCore.Blazor.Hosting;
using System.Threading.Tasks;

namespace BlazorApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }

        private static WebAssemblyHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args)
                //不知道为什么，请求成功发出，数据正确读取到，但是 IConfigurationBuilder.Build() 好像没有正常运行，估计是预览版还没有写好配置构建
                //.ConfigureAppConfiguration(async (hostContext, configBuilder) =>
                //{
                //    configBuilder.AddHttpJsonStream(await (await hostContext.Http.GetAsync("/blazor/configuration/appsettings.json")).Content.ReadAsStreamAsync());
                //})
                .UseStartup<Startup>();

            builder.RootComponents.Add<App>("app");

            return builder;
        }
    }
}
