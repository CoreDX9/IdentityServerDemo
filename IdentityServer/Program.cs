#if !DEBUG
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
#endif

using System;
using System.Linq;
using System.Reflection;
using Domain.Security;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if !DEBUG
            //通过特殊手段运行应用可能导致工作目录与程序文件所在目录不一致，需要调整，否则配置文件和其他数据无法加载（仅限发布模式，调试模式修改工作目录也可能导致配置和其他数据无法加载）
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
#endif
            var host = CreateWebHostBuilder(args).Build();
            EnsureRequestHandlerIdentificationDoNotHaveDuplicate(host.Services);
            SeedData.EnsureSeedData(host.Services);
            host.Run();
        }

        /// <summary>
        /// 检查确保请求处理方法上的RequestHandlerIdentificationAttribute的UniqueKey没有重复的
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        private static void EnsureRequestHandlerIdentificationDoNotHaveDuplicate(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var actionDescriptorCollectionProvider =
                    scope.ServiceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();
                var pageLoader = scope.ServiceProvider.GetRequiredService<IPageLoader>();

                var duplicateOfRequestHandlerIdentifications =
                    actionDescriptorCollectionProvider.ActionDescriptors.Items
                        .OfType<ControllerActionDescriptor>()
                        .Select(action => action.MethodInfo)
                        .Concat(
                            actionDescriptorCollectionProvider.ActionDescriptors.Items
                                .OfType<PageActionDescriptor>()
                                .Select(page => pageLoader.Load(page).HandlerMethods.Select(handler => handler.MethodInfo))
                                .SelectMany(vb => vb))
                        .Where(m => m.GetCustomAttribute<RequestHandlerIdentificationAttribute>() != null)
                        .Select(m => m.GetCustomAttribute<RequestHandlerIdentificationAttribute>().UniqueKey)
                        .GroupBy(name => name)
                        .Where(group => group.Count() > 1)
                        .Select(group => new { key = group.Key, count = group.Count() });

                if (duplicateOfRequestHandlerIdentifications.Any())
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<RequestHandlerIdentificationAttribute>>();
                    var msg = $"\"{string.Join('、', duplicateOfRequestHandlerIdentifications.Select(x => x.key))}\" 在 RequestHandlerIdentificationAttribute 中重复出现。";
                    var exception = new Exception(msg);
                    logger.LogError(exception, msg);
                    throw exception;
                }
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>();

#if !DEBUG
            host.UseKestrel(SetHost);
#endif

            return host;
        }

#if !DEBUG
        /// <summary>
        /// 配置Kestrel
        /// </summary>
        /// <param name="options"></param>
        private static void SetHost(Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions options)
        {
            var configuration = (IConfiguration)options.ApplicationServices.GetService(typeof(IConfiguration));
            var host = configuration.GetSection("RafHost").Get<Host>();//依据Host类反序列化appsettings.json中指定节点
            foreach (var endpointKvp in host.Endpoints)
            {
                //var endpointName = endpointKvp.Key;
                var endpoint = endpointKvp.Value;//获取appsettings.json的相关配置信息
                if (!endpoint.IsEnabled)
                {
                    continue;
                }

                var address = IPAddress.Parse(endpoint.Address);
                options.Listen(address, endpoint.Port, opt =>
                {
                    if (endpoint.Certificate != null)//证书不为空使用UserHttps
                    {
                        switch (endpoint.Certificate.Source)
                        {
                            case "File":
                                opt.UseHttps($@"{Directory.GetCurrentDirectory()}\{endpoint.Certificate.Path}", endpoint.Certificate.Password);
                                break;
                            default:
                                throw new NotImplementedException($"文件 {endpoint.Certificate.Source}还没有实现");
                        }

                        //opt.UseConnectionLogging();
                    }
                });

                options.UseSystemd();
            }
        }

        /// <summary>
        /// 待反序列化节点
        /// </summary>
        public class Host
        {
            /// <summary>
            /// appsettings.json字典
            /// </summary>
            public Dictionary<string, Endpoint> Endpoints { get; set; }
        }

        /// <summary>
        /// 终结点
        /// </summary>
        public class Endpoint
        {
            /// <summary>
            /// 是否启用
            /// </summary>
            public bool IsEnabled { get; set; }

            /// <summary>
            /// ip地址
            /// </summary>
            public string Address { get; set; }

            /// <summary>
            /// 端口号
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// 证书
            /// </summary>
            public Certificate Certificate { get; set; }
        }

        /// <summary>
        /// 证书类
        /// </summary>
        public class Certificate
        {
            /// <summary>
            /// 源
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// 证书路径
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// 证书密钥
            /// </summary>
            public string Password { get; set; }
        }
#endif
    }
}
