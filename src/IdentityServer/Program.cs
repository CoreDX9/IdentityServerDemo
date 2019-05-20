#if !DEBUG
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Domain.Security;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Repository.EntityFrameworkCore;
using Util.TypeExtensions;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var mutex = new Mutex(true, "IdentityServerDemo",out var isUniqueInstanceOfApplication);
            if (!isUniqueInstanceOfApplication)
            {
                Console.WriteLine("已经有一个服务实例正在运行。");
                Console.WriteLine("按任意键退出……");
                Console.ReadKey();
                Environment.Exit(-2);
            }
#if !DEBUG
            //通过特殊手段运行应用可能导致工作目录与程序文件所在目录不一致，需要调整，否则配置文件和其他数据无法加载（仅限发布模式，调试模式修改工作目录也可能导致配置和其他数据无法加载）
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
#endif
            var host = CreateWebHostBuilder(args).Build();
            SeedData.EnsureSeedData(host.Services);
            EnsureRequestHandlerIdentificationDoNotHaveDuplicate(host.Services);
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
                #region 检查特性重复

                var actionDescriptorCollectionProvider =
                    scope.ServiceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();
                var pageLoader = scope.ServiceProvider.GetRequiredService<IPageLoader>();

                var requestHandlerIdentifications = actionDescriptorCollectionProvider.ActionDescriptors.Items
                    .OfType<ControllerActionDescriptor>()
                    .Select(action => action.MethodInfo)
                    .Concat(
                        actionDescriptorCollectionProvider.ActionDescriptors.Items
                            .OfType<PageActionDescriptor>()
                            .Select(page => pageLoader.Load(page).HandlerMethods.Select(handler => handler.MethodInfo))
                            .SelectMany(methods => methods));

                var duplicateOfRequestHandlerIdentifications =
                    requestHandlerIdentifications
                        .Where(m => m.GetCustomAttribute<RequestHandlerIdentificationAttribute>() != null)
                        .GroupBy(m => m.GetCustomAttribute<RequestHandlerIdentificationAttribute>().UniqueKey);

                var duplicateList = new List<string>();
                foreach (var group in duplicateOfRequestHandlerIdentifications.Where(group => group.Count() > 1))
                {
                    var ng = group.AsEnumerable().GroupBy(m => new {m.DeclaringType, Name = m.ToString()});
                    if (ng.Count() > 1)
                    {
                        duplicateList.Add(group.Key);
                    }
                }

                if (duplicateList.Count > 0)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<RequestHandlerIdentificationAttribute>>();
                    var msg = $"\"{string.Join('、', duplicateList)}\" 在 RequestHandlerIdentificationAttribute 中重复出现。";
                    var exception = new Exception(msg);
                    logger.LogError(exception, msg);
                    throw exception;
                }

                #endregion

                using (var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    #region 检查数据库记录重复

                    //查出所有请求授权规则
                    var requestAuthorizationRules =  context.RequestAuthorizationRules.AsNoTracking().ToList();
                    //按IdentificationKey分组并按最后修改时间降序为字典
                    var a = requestAuthorizationRules
                        .Where(r => !r.IdentificationKey.IsNullOrEmpty())
                        .GroupBy(r => r.IdentificationKey)
                        .ToDictionary(r => r.Key, r => r.OrderByDescending(r1 => r1.LastModificationTime).ToArray());
                    //删除每组第一个以外的元素
                    foreach (var requestAuthorizationRulese in a)
                    {
                        if (requestAuthorizationRulese.Value.Length > 1)
                        {
                            throw new Exception($"RequestAuthorizationRule的Key: \"{requestAuthorizationRulese.Key}\" 出现重复。");
                        }
                        //context.RequestAuthorizationRules.RemoveRange(requestAuthorizationRulese.Value.Skip(1));
                    }
                    //按类型全名和方法签名构造Key分组并按最后修改时间降序为字典
                    var b = requestAuthorizationRules
                        .GroupBy(r=> $"{r.TypeFullName}/{r.HandlerMethodSignature}")
                        .ToDictionary(r => r.Key, r => r.OrderByDescending(r1 => r1.LastModificationTime).ToArray());
                    //删除每组第一个以外的元素
                    foreach (var requestAuthorizationRulese in b)
                    {
                        if (requestAuthorizationRulese.Value.Length > 1)
                        {
                            throw new Exception($"RequestAuthorizationRule的类型信息: \"{requestAuthorizationRulese.Key}\" 出现重复。");
                        }
                        //context.RequestAuthorizationRules.RemoveRange(requestAuthorizationRulese.Value.Skip(1));
                    }
                    //保存结果（确保每个IdentificationKey和请求处理器只有一条记录）
                    //context.SaveChanges();

                    #endregion

                    requestAuthorizationRules = context.RequestAuthorizationRules.ToList();
                    //找出有RequestHandlerIdentification特性的记录并同步类型和签名记录

                    foreach (var group in duplicateOfRequestHandlerIdentifications)
                    {
                        //查找IdentificationKey与RequestHandlerIdentification特性匹配的记录
                        var f = requestAuthorizationRules.SingleOrDefault(r => r.IdentificationKey == group.Key);
                        if (f != null)//更新类型信息
                        {
                            if (f.TypeFullName != group.First().DeclaringType.FullName)
                            {
                                f.TypeFullName = group.First().DeclaringType.FullName;
                            }

                            if (f.HandlerMethodSignature != group.First().ToString())
                            {
                                f.HandlerMethodSignature = group.First().ToString();
                            }

                            //删除类型匹配的无效记录（例如将一个RequestHandlerIdentification特性的值挪用到另一个请求处理器时会出现此情况）
                            var ff = requestAuthorizationRules.SingleOrDefault(r =>
                                r != f && r.TypeFullName == group.First().DeclaringType.FullName &&
                                r.HandlerMethodSignature == group.First().ToString());
                            if (ff != null)
                            {
                                context.RequestAuthorizationRules.Remove(ff);
                            }
                            continue;
                        }
                        //查找类型信息与请求处理器匹配的记录
                        f = requestAuthorizationRules.SingleOrDefault(r => r.TypeFullName == group.First().DeclaringType.FullName && r.HandlerMethodSignature == group.First().ToString());
                        if (f != null)//更新IdentificationKey
                        {
                            if (f.IdentificationKey != group.Key)
                            {
                                f.IdentificationKey = group.Key;
                            }
                        }
                    }
                    //保存结果
                    context.SaveChanges();

                    #region 复查重复情况

                    //复查一遍有没有重复的（理论上说任何情况都不应该在复查时出现重复）
                    //查出所有请求授权规则
                    requestAuthorizationRules = context.RequestAuthorizationRules.AsNoTracking().ToList();
                    //按IdentificationKey分组并按最后修改时间降序为字典
                    a = requestAuthorizationRules
                        .Where(r => !r.IdentificationKey.IsNullOrEmpty())
                        .GroupBy(r => r.IdentificationKey)
                        .ToDictionary(r => r.Key, r => r.OrderByDescending(r1 => r1.LastModificationTime).ToArray());
                    //删除每组第一个以外的元素
                    foreach (var requestAuthorizationRulese in a)
                    {
                        if (requestAuthorizationRulese.Value.Length > 1)
                        {
                            throw new Exception($"RequestAuthorizationRule的Key: \"{requestAuthorizationRulese.Key}\" 出现重复。");
                        }
                    }
                    //按类型全名和方法签名构造Key分组并按最后修改时间降序为字典
                    b = requestAuthorizationRules
                        .GroupBy(r => $"{r.TypeFullName}/{r.HandlerMethodSignature}")
                        .ToDictionary(r => r.Key, r => r.OrderByDescending(r1 => r1.LastModificationTime).ToArray());
                    //删除每组第一个以外的元素
                    foreach (var requestAuthorizationRulese in b)
                    {
                        if (requestAuthorizationRulese.Value.Length > 1)
                        {
                            throw new Exception($"RequestAuthorizationRule的类型信息: \"{requestAuthorizationRulese.Key}\" 出现重复。");
                        }
                    }

                    #endregion
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
