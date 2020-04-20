#if !DEBUG
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
#endif

using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Application.EntityFrameworkCore.IdentityServer;
using CoreDX.Application.EntityFrameworkCore.IdentityServer.Admin;
using CoreDX.Domain.Entity.Identity;
using IdentityServer.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Layouts;
using NLog.Web;
using Serilog;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region 确保只有一个实例在运行

            var mutex = new Mutex(true, "IdentityServerDemo", out var isUniqueInstanceOfApplication);
            if (!isUniqueInstanceOfApplication)
            {
                Console.WriteLine("已经有一个实例正在运行。");
                Console.WriteLine("按任意键或3秒后自动退出……");
                await Task.Run(() =>
                {
                    Thread.Sleep(3000);
                    Environment.Exit(-3);
                });
                Console.ReadKey();
                Environment.Exit(-2);
            }

            #endregion

            #region 确保发行版的工作目录正确

#if !DEBUG
            //通过特殊手段运行应用可能导致工作目录与程序文件所在目录不一致，需要调整，否则配置文件和其他数据无法加载（仅限发布模式，调试模式修改工作目录也可能导致配置和其他数据无法加载）
            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            Directory.SetCurrentDirectory(pathToContentRoot);
#endif

            #endregion

            var host = CreateHostBuilder(args).Build();
            await EnsureSeedDataAsync(host);
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureAppConfiguration((hostContext, configBuilder) =>
                {
                    configBuilder
                        .AddJsonConfiguration("serilog", hostContext.HostingEnvironment.EnvironmentName)
                        .AddJsonConfiguration("identitydata", hostContext.HostingEnvironment.EnvironmentName)
                        .AddJsonConfiguration("identityserverdata", hostContext.HostingEnvironment.EnvironmentName)
                        .AddJsonConfiguration("appRateLimitSettings", hostContext.HostingEnvironment.EnvironmentName)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);

                    //if (hostContext.HostingEnvironment.IsDevelopment())
                    //{
                    //    configBuilder.AddUserSecrets<Startup>();
                    //}
                });

            using var loggingConfigFile = File.OpenText("LoggingModule.json");
            var jsonOptions = new JsonDocumentOptions() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
            var loggingModule = JsonDocument.Parse(loggingConfigFile.BaseStream, jsonOptions).RootElement.GetProperty("Name").GetString();
            switch (loggingModule)
            {
                case "NLog":
                    builder.ConfigureLogging((hostContext, logging) =>
                    {
                        var path = $@"{hostContext.HostingEnvironment.ContentRootPath}\NLog.{hostContext.HostingEnvironment.EnvironmentName}.config";
                        var nlogConfig = File.Exists(path)
                            ? path
                            : $@"{hostContext.HostingEnvironment.ContentRootPath}\NLog.config";

                        var configuration = LogManager.LoadConfiguration(nlogConfig).Configuration;
                        configuration.Variables.Add("rootPath", new SimpleLayout(hostContext.HostingEnvironment.ContentRootPath));
                        logging.AddNLog(configuration);
                    })
                        .UseNLog(new NLogAspNetCoreOptions() { ShutdownOnDispose = true, IncludeScopes = true, RegisterHttpContextAccessor = true });
                    break;
                case "Serilog":
                    goto default;
                default:
                    builder.UseSerilog((hostContext, loggerConfig) =>
                    {
                        //var logDB = @"Server=...";
                        //var logTable = "Logs";
                        //var opts = new Serilog.Sinks.MSSqlServer.ColumnOptions();

                        loggerConfig
                            .ReadFrom.Configuration(hostContext.Configuration)
                            .Enrich.WithProperty("ApplicationName", hostContext.HostingEnvironment.ApplicationName);
                    });
                    break;
            }

            builder.UseWindowsService()//如果将应用安装为Windows服务，会自动以服务方式运行，否则继续以控制台方式运行
            .ConfigureWebHostDefaults(webBuilder =>
            {
#if !DEBUG
                    webBuilder.ConfigureKestrel(SetHost);
#endif
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                });

            return builder;
        }

        public static IHostBuilder CreateHostBuilderP(string[] args)
        {
            return CreateHostBuilder(args);
        }

        public static async Task EnsureSeedDataAsync(IHost host)
        {
            await DbMigrationHelpers
                .EnsureSeedData<IdentityServerConfigurationDbContext, ApplicationIdentityDbContext,
                    IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext,
                    ApplicationUser, ApplicationRole, int>(host);
        }

#if !DEBUG
        /// <summary>
        /// 配置Kestrel
        /// </summary>
        /// <param name="options"></param>
        private static void SetHost(Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions options)
        {
            var configuration = (IConfiguration)options.ApplicationServices.GetService(typeof(IConfiguration));
            var host = configuration.GetSection("RafHost").Get<HostInfo>();//依据Host类反序列化appsettings.json中指定节点
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
        public class HostInfo
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
