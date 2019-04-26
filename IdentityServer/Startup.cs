using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using AutoMapper;
using Domain.Identity;
using Extensions.Logging.File;
using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityServer.CustomMiddlewares;
using IdentityServer.CustomServices;
using IdentityServer.Extensions;
using IdentityServer.Hubs;
using IdentityServer4.Configuration;
using Joonasw.AspNetCore.SecurityHeaders;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository.EntityFrameworkCore;
using Repository.RabbitMQ;
using StackExchange.Redis;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public IServiceCollection Services { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // 异步控制器动作或Razor页面动作返回void可能导致从DI容器获取的efcore或者其他对象被释放，返回Task可以避免这个问题
        public void ConfigureServices(IServiceCollection services)
        {
            Services = services;
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                //GDPR支持配置
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //注入视图渲染服务
            services.AddTransient<RazorViewToStringRenderer>();

            //注入AutoMapper服务
            services.AddAutoMapper();

            //注入响应压缩服务（gzip）
            services.AddResponseCompression();

            //注入SignalR服务
            var signalRServer = services.AddSignalR();
            //在没有Redis的机器上运行项目设置false
            if (Configuration.GetValue("UseRedisForSignalR", false))
            {
                var redisConfig = Configuration.GetSection("RedisForSignalR");

                signalRServer.AddRedis(options =>
                {
                    options.ConnectionFactory = async writer =>
                    {
                        var config = new ConfigurationOptions
                        {
                            AbortOnConnectFail = false
                        };
                        config.EndPoints.Add(redisConfig.GetValue<string>("Host"), redisConfig.GetValue<int>("Port"));
                        //config.SetDefaultPorts();
                        config.Password = redisConfig.GetValue<string>("Password");
                        Console.WriteLine("SignalR服务正在连接Redis服务器……");
                        var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
                        connection.ConnectionFailed += (_, e) =>
                        {
                            Console.WriteLine("SignalR服务连接Redis服务器连接失败");
                        };

                        if (!connection.IsConnected)
                        {
                            Console.WriteLine("SignalR服务未连接到Redis服务器");
                        }

                        if (connection.IsConnected)
                        {
                            Console.WriteLine("SignalR服务已连接到Redis服务器");
                        }

                        return connection;
                    };
                });
            }

            //注入应用数据保护服务
            services.AddDataProtection()
                .SetApplicationName("IdentityServerDemo")
                .PersistKeysToFileSystem(new DirectoryInfo($@"{Environment.ContentRootPath}\App_Data\DataProtectionKey"));

            //在没有RabbitMQ的机器上运行项目设置false
            if (Configuration.GetValue("UseEntityHistory", false))
            {
                //注入实体历史记录服务，供ApplicationIdentityDbContext用
                services.AddEntityHistoryRecorder(Configuration);
            }

            var useInMemoryDatabase = Configuration.GetValue("UseInMemoryDatabase", false);
            var connectionString = string.Empty;
            var migrationsAssemblyName = string.Empty;
            InMemoryDatabaseRoot inMemoryDatabaseRoot = null;
            //注入EF上下文
            if (useInMemoryDatabase)
            {
                inMemoryDatabaseRoot = new InMemoryDatabaseRoot();
                services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("IdentityServerDb-InMemory", inMemoryDatabaseRoot);
                    });
            }
            else
            {
                connectionString = Configuration.GetConnectionString("IdentityServerDbContextConnection");

                //重定向数据库文件（默认文件在用户文件夹，改到项目内部文件夹方便管理）
                if (Environment.IsDevelopment())
                {
                    connectionString += $@";AttachDbFileName={Environment.ContentRootPath}\App_Data\Database\IdentityServerDb-Dev.mdf";
                }
                if (Environment.IsProduction())
                {
                    connectionString += $@";AttachDbFileName={Environment.ContentRootPath}\App_Data\Database\IdentityServerDb-Production.mdf";
                }

                //迁移程序集名
                migrationsAssemblyName = "DbMigration";

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(connectionString, b =>
                    {
                        b.MigrationsAssembly(migrationsAssemblyName);
                        b.EnableRetryOnFailure(3);
                    });
                });
            }
            
            //注入Identity服务（使用EF存储，在EF上下文之后注入）
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    // Password settings.
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequiredUniqueChars = 0;

                    // Lockout settings.
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings.
                    //options.User.AllowedUserNameCharacters =
                    //    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = true;

                    //启用Identity隐私数据保护
                    //一旦加密，必须保证密钥安全，密钥丢失将无法解密数据
                    //隐私加密会修改模型配置，可能需要运行迁移，并且会改变隐私数据的存储格式
                    //在有数据的情况下修改配置必须自行对隐私数据进行转换
                    //所有受保护的隐私字段通过Linq To EF Core只能进行精确匹配查询
                    //模糊查询只能全表或受保护字段载入内存再进行内存筛选
                    options.Stores.ProtectPersonalData = true;
                })
                .AddRoles<ApplicationRole>()
                .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                //注入Identity隐私数据保护服务（启用隐私数据保护后必须注入，和上面那个应用数据保护服务不一样，这个是给IdentityDbContext用的，上面那个是给cookies之类加密用的）
                .AddPersonalDataProtection<AesProtector, AesProtectorKeyRing>()
                .AddDefaultTokenProviders();

            //配置Identity跳转链接
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
            });

            //配置本地化数据服务存储
            if (useInMemoryDatabase)
            {
                services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<LocalizationModelContext>(
                        options => { options.UseInMemoryDatabase("IdentityServerDb-InMemory", inMemoryDatabaseRoot); },
                        ServiceLifetime.Singleton,
                        ServiceLifetime.Singleton);
            }
            else
            {
                services.AddDbContext<LocalizationModelContext>(options =>
                    {
                        options.UseSqlServer(connectionString, b =>
                        {
                            b.MigrationsAssembly(migrationsAssemblyName);
                            b.EnableRetryOnFailure(3);
                        });
                    },
                    ServiceLifetime.Singleton,
                    ServiceLifetime.Singleton);
            }

            //注入修改后的本地化服务工厂（SqlStringLocalizerFactory在IViewLocalizer中使用时无法自动创建记录）
            services.AddSingleton<IStringLocalizerFactory, MySqlStringLocalizerFactory>();
            //注入基于Sql数据的本地化服务，需要先注入LocalizationModelContext
            services.AddSqlLocalization(options => options.UseSettings(true, false, true, true));
            //注入请求处理器信息获取服务
            services.AddSingleton<IRequestHandlerInfo, RequestHandlerInfo>();

            //注入MVC相关服务
            services.AddMvc(options =>//在这里添加的过滤器可以使用构造方法依赖注入获取任何已经注册到服务容器的服务
                {
                    //options.Filters.Add<MyAsyncPageFilter>();
                    //options.Filters.Add<MyAuthorizeAttribute>();
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());//自动对控制器的post，put，delete，patch请求进行保护
                })
                //注入FluentValidation服务
                .AddFluentValidation(fv =>
                {
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
                    fv.ImplicitlyValidateChildProperties = true;
                })
                //注入视图本地化服务
                .AddViewLocalization()
                //注入数据注解本地化服务
                .AddDataAnnotationsLocalization()
                //设定兼容性
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //注入FluentValidation验证器
            services.AddTransient<IValidator<Pages.FluentValidationDemo.IndexModel.A>, Pages.FluentValidationDemo.IndexModel.AValidator>();

            //AssemblyScanner.FindValidatorsInAssemblyContaining<Startup>().ForEach(pair => {
            //    // RegisterValidatorsFromAssemblyContaing does this:
            //    services.Add(ServiceDescriptor.Transient(pair.InterfaceType, pair.ValidatorType));
            //    // Also register it as its concrete type as well as the interface type
            //    services.Add(ServiceDescriptor.Transient(pair.ValidatorType, pair.ValidatorType));
            //});

            //配置请求本地化选项
            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("zh-CN"),
                        new CultureInfo("en-US")
                    };

                    options.DefaultRequestCulture = new RequestCulture(culture: "zh-CN", uiCulture: "zh-CN");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });

            //注入电子邮件发送服务
            services.AddScoped<IEmailSender, EmailSender>();

            //结合EFCore生成IdentityServer4数据库迁移命令详情见Repository项目说明文档
            //项目工程文件最后添加 <ItemGroup><DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" /></ItemGroup>
            //添加IdentityServer4对EFCore数据库的支持
            //但是这里需要初始化数据 默认生成的数据库中是没有配置数据

            //注入IdentityServer4服务
            var id4 = services.AddIdentityServer(options =>
                {
                    //活动事件 允许配置是否应该将哪些事件提交给注册的事件接收器
                    options.Events = new EventsOptions
                    {
                        RaiseErrorEvents = true,
                        RaiseFailureEvents = true,
                        RaiseInformationEvents = true,
                        RaiseSuccessEvents = true
                    };

                    //设置认证
                    options.Authentication = new AuthenticationOptions
                    {
                        CheckSessionCookieName = "idr.Cookies",  //用于检查会话端点的cookie的名称
                        CookieLifetime = new TimeSpan(1, 0, 0), //身份验证Cookie生存期（仅在使用IdentityServer提供的Cookie处理程序时有效）
                        CookieSlidingExpiration = true, //指定cookie是否应该滑动（仅在使用IdentityServer提供的cookie处理程序时有效）
                        RequireAuthenticatedUserForSignOutMessage = true //指示是否必须对用户进行身份验证才能接受参数以结束会话端点。默认为false
                    };

                    //允许设置各种协议参数（如客户端ID，范围，重定向URI等）的长度限制
                    //options.InputLengthRestrictions = new IdentityServer4.Configuration.InputLengthRestrictions
                    //{
                    //    //可以看出下面很多参数都是对长度的限制 
                    //    AcrValues = 100,
                    //    AuthorizationCode = 100,
                    //    ClientId = 100,
                    //    /*
                    //    ..
                    //    ..
                    //    ..
                    //    */
                    //    ClientSecret = 1000
                    //};

                    //用户交互页面定向设置处理
                    options.UserInteraction = new UserInteractionOptions
                    {
                        LoginUrl = "/IdentityServer/Account/Login", //【必备】登录地址  会覆盖全局未授权跳转地址替换掉aspnet Identity内置登录页，a标签与302跳转不受影响
                        LogoutUrl = "/IdentityServer/Account/Logout", //【必备】退出地址 
                        ConsentUrl = "/IdentityServer/Consent", //【必备】允许授权同意页面地址
                        ErrorUrl = "/IdentityServer/Home/Error", //【必备】错误页面地址
                        LoginReturnUrlParameter = "returnUrl", //【必备】设置传递给登录页面的返回URL参数的名称。默认为returnUrl 
                        LogoutIdParameter = "logoutId", //【必备】设置传递给注销页面的注销消息ID参数的名称。缺省为logoutId 
                        ConsentReturnUrlParameter = "returnUrl", //【必备】设置传递给同意页面的返回URL参数的名称。默认为returnUrl
                        ErrorIdParameter = "errorId", //【必备】设置传递给错误页面的错误消息ID参数的名称。缺省为errorId
                        CustomRedirectReturnUrlParameter = "returnUrl", //【必备】设置从授权端点传递给自定义重定向的返回URL参数的名称。默认为returnUrl
                        CookieMessageThreshold = 5 //【必备】由于浏览器对Cookie的大小有限制，设置Cookies数量的限制，有效的保证了浏览器打开多个选项卡，一旦超出了Cookies限制就会清除以前的Cookies值
                    };

                    //缓存参数处理  缓存起来提高了效率 不用每次从数据库查询
                    options.Caching = new CachingOptions
                    {
                        ClientStoreExpiration = new TimeSpan(1, 0, 0), //设置Client客户端存储加载的客户端配置的数据缓存的有效时间 
                        ResourceStoreExpiration = new TimeSpan(1, 0, 0), // 设置从资源存储加载的身份和API资源配置的缓存持续时间
                        CorsExpiration = new TimeSpan(1, 0, 0) //设置从资源存储的跨域请求数据的缓存时间
                    };

                    //IdentityServer支持一些端点的CORS。底层CORS实现是从ASP.NET Core提供的，因此它会自动注册在依赖注入系统中
                    //options.Cors = new CorsOptions
                    //{
                    //    CorsPaths = { "/" }, //支持CORS的IdentityServer中的端点。默认为发现，用户信息，令牌和撤销终结点
                    //    CorsPolicyName =
                    //        "default", //【必备】将CORS请求评估为IdentityServer的CORS策略的名称（默认为"IdentityServer4"）。处理这个问题的策略提供者是ICorsPolicyService在依赖注入系统中注册的。如果您想定制允许连接的一组CORS原点，则建议您提供一个自定义的实现ICorsPolicyService
                    //    PreflightCacheDuration =
                    //        new TimeSpan(1, 0, 0) //可为空的<TimeSpan>，指示要在预检Access-Control-Max-Age响应标题中使用的值。默认为空，表示在响应中没有设置缓存头
                    //};
                    options.Cors.PreflightCacheDuration = new TimeSpan(1, 0, 0);
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential();

            //配置IdentityServer4存储
            if (useInMemoryDatabase)
            {
                id4.AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseInMemoryDatabase("IdentityServerDb-InMemory", inMemoryDatabaseRoot);
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseInMemoryDatabase("IdentityServerDb-InMemory", inMemoryDatabaseRoot);

                        options.EnableTokenCleanup = true;
                    });
            }
            else
            {
                // this adds the config data from DB (clients, resources)
                id4.AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseSqlServer(connectionString,
                                sql => sql.MigrationsAssembly(migrationsAssemblyName));
                    })
                    // this adds the operational data from DB (codes, tokens, consents)
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseSqlServer(connectionString,
                                sql => sql.MigrationsAssembly(migrationsAssemblyName));

                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                        // options.TokenCleanupInterval = 15; // frequency in seconds to cleanup stale grants. 15 is useful during debugging
                    });
            }

            //注入身份验证服务
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";
                    options.ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh";
                })
                .AddOpenIdConnect("oidc", "OpenID Connect", options =>
                {
                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "implicit";
                    options.SaveTokens = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });

            //注入跨域访问服务
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.WithOrigins("https://localhost:5003", "https://localhost:5005", "https://localhost:5007")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }));

            //注入反CSRF服务并配置请求头名称
            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

            //注入CSP（内容安全策略）服务
            services.AddCsp();

            //注入Hsts（传输安全）服务
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                //options.ExcludedHosts.Add("localhost:5000");
            });

            //services.AddHttpsRedirection(options =>
            //{
            //    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            //    options.HttpsPort = 5001;
            //});

            //注入开发环境文件夹浏览服务
            if (Environment.IsDevelopment())
            {
                services.AddDirectoryBrowser();
            }

            //注入（工厂方式激活的）自定义中间件服务
            services.AddScoped<AntiforgeryTokenGenerateMiddleware>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.Map("/allservices", builder => builder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync($"<h1>所有服务{Services.Count}个</h1><table><thead><tr><th>类型</th><th>生命周期</th><th>Instance</th></tr></thead><tbody>");
                    foreach (var svc in Services)
                    {
                        await context.Response.WriteAsync("<tr>");
                        await context.Response.WriteAsync($"<td>{svc.ServiceType.FullName}</td>");
                        await context.Response.WriteAsync($"<td>{svc.Lifetime}</td>");
                        await context.Response.WriteAsync($"<td>{svc.ImplementationType?.FullName}</td>");
                        await context.Response.WriteAsync("</tr>");
                    }
                    await context.Response.WriteAsync("</tbody></table>");
                }));
            }
            //添加文件日志
            loggerFactory.AddFile(Configuration.GetSection("FileLogging"));

            //配置FluentValidation的本地化
            app.ConfigLocalizationFluentValidation();

            //注册管道是有顺序的

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //检查到相应配置启用https跳转
            if (Configuration.GetValue("UseHttpsRedirection", false) &&
                (Configuration.GetSection("RafHost").GetSection("Endpoints").GetSection("Https")
                     .GetValue("IsEnabled", false) || Environment.IsDevelopment()))
            {
                //app.UseHsts();
                HstsBuilderExtensions.UseHsts(app);
                //注册强制Https跳转到管道
                app.UseHttpsRedirection();
            }

            //注册响应压缩到管道
            app.UseResponseCompression();

            //注册内容安全策略到管道
            // Content Security Policy
            app.UseCsp(csp =>
            {
                // If nothing is mentioned for a resource class, allow from this domain
                csp.ByDefaultAllow
                    .FromSelf();

                // Allow JavaScript from:
                csp.AllowScripts
                    .FromSelf() //This domain
                    .AllowUnsafeInline()
                    .AllowUnsafeEval()
                    .From("localhost:5000") //These two domains
                    .From("localhost:5001")
                    .From("localhost:5002")
                    .From("localhost:5003")
                    .From("localhost:5004")
                    .From("localhost:5005")
                    .From("ajax.aspnetcdn.com")
                    .From("cdnjs.cloudflare.com");
                //.AddNonce();//此项与AllowUnsafeInline冲突，会被AllowUnsafeInline选项覆盖

                // CSS allowed from:
                csp.AllowStyles
                    .FromSelf()
                    .AllowUnsafeInline()
                    .From("localhost:5000") //These two domains
                    .From("localhost:5001")
                    .From("localhost:5002")
                    .From("localhost:5003")
                    .From("localhost:5004")
                    .From("localhost:5005")
                    .From("ajax.aspnetcdn.com")
                    .From("fonts.googleapis.com")
                    .From("cdnjs.cloudflare.com");
                //.AddNonce();//此项与AllowUnsafeInline冲突，会被AllowUnsafeInline选项覆盖

                csp.AllowImages
                    .FromSelf()
                    .DataScheme()
                    .From("localhost:5000") //These two domains
                    .From("localhost:5001")
                    .From("localhost:5002")
                    .From("localhost:5003")
                    .From("localhost:5004")
                    .From("localhost:5005")
                    .From("ajax.aspnetcdn.com");

                // HTML5 audio and video elemented sources can be from:
                csp.AllowAudioAndVideo
                    .FromNowhere();//Nowhere, no media allowed

                // Contained iframes can be sourced from:
                csp.AllowFrames
                    .FromSelf(); 

                // Allow AJAX, WebSocket and EventSource connections to:
                csp.AllowConnections
                    .ToSelf()
                    .To("ws://localhost:5000")
                    .To("wss://localhost:5001")
;

                // Allow fonts to be downloaded from:
                csp.AllowFonts
                    .FromSelf()
                    .From("fonts.gstatic.com")
                    .From("ajax.aspnetcdn.com");

                // Allow object, embed, and applet sources from:
                csp.AllowPlugins
                    .FromNowhere();

                // Allow other sites to put this in an iframe?
                csp.AllowFraming
                    .FromAnywhere(); // Block framing on other sites, equivalent to X-Frame-Options: DENY

                if (env.IsDevelopment())
                {
                    // Do not block violations, only report
                    // This is a good idea while testing your CSP
                    // Remove it when you know everything will work
                    //csp.SetReportOnly();
                    // Where should the violation reports be sent to?
                    //csp.ReportViolationsTo("/csp-report");
                }

                // Do not include the CSP header for requests to the /api endpoints
                //csp.OnSendingHeader = context =>
                //{
                //    context.ShouldNotSend = context.HttpContext.Request.Path.StartsWithSegments("/api");
                //    return Task.CompletedTask;
                //};
            });

            //注册请求本地化到管道
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            //注册默认404页面到管道
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode != (int)HttpStatusCode.NotFound)
                {
                    return;
                }

                PathString pathString = "/Home/NotFound";
                QueryString queryString = new QueryString();
                PathString originalPath = context.HttpContext.Request.Path;
                QueryString originalQueryString = context.HttpContext.Request.QueryString;
                context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature()
                {
                    OriginalPathBase = context.HttpContext.Request.PathBase.Value,
                    OriginalPath = originalPath.Value,
                    OriginalQueryString = (originalQueryString.HasValue ? originalQueryString.Value : null)
                });
                context.HttpContext.Request.Path = pathString;
                context.HttpContext.Request.QueryString = queryString;
                try
                {
                    await context.Next(context.HttpContext);
                }
                finally
                {
                    context.HttpContext.Request.QueryString = originalQueryString;
                    context.HttpContext.Request.Path = originalPath;
                    context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(null);
                }
            });

            //注册开发环境文件浏览器
            if (Environment.IsDevelopment())
            {
                var dir = new DirectoryBrowserOptions();
                dir.FileProvider = new PhysicalFileProvider(Environment.ContentRootPath);
                dir.RequestPath = "/dir";
                app.UseDirectoryBrowser(dir);

                var contentTypeProvider = new FileExtensionContentTypeProvider();
                contentTypeProvider.Mappings.Add(".log", "text/plain");

                var devStaticFileOptions = new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Environment.ContentRootPath),
                    RequestPath = "/dir",
                    ServeUnknownFileTypes = true,
                    DefaultContentType = "application/octet-stream",
                    ContentTypeProvider = contentTypeProvider
                };

                app.UseStaticFiles(devStaticFileOptions);
            }

            //注册开发环境的npm和bower资源
            if (Environment.IsDevelopment())
            {
                var npmContentTypeProvider = new FileExtensionContentTypeProvider();
                var npmStaticFileOptions = new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Environment.ContentRootPath + "/node_modules"),
                    RequestPath = "/npm",
                    ServeUnknownFileTypes = false,
                    ContentTypeProvider = npmContentTypeProvider
                };

                app.UseStaticFiles(npmStaticFileOptions);

                var bowerContentTypeProvider = new FileExtensionContentTypeProvider();
                var bowerStaticFileOptions = new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Environment.ContentRootPath + "/bower_components"),
                    RequestPath = "/bower",
                    ServeUnknownFileTypes = false,
                    ContentTypeProvider = bowerContentTypeProvider
                };
                
                app.UseStaticFiles(bowerStaticFileOptions);
            }

            //注册静态文件到管道（wwwroot文件夹）
            app.UseStaticFiles();

            //注册Cookie策略到管道（GDPR）
            app.UseCookiePolicy();

            //注册跨域策略到管道
            app.UseCors("CorsPolicy");

            //注册IdentityServer4到管道
            app.UseIdentityServer();

            //注册SignalR到管道
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });

            //注册自定义中间件到管道
            app.UseAntiforgeryTokenGenerateMiddleware();

            //注册MVC到管道
            app.UseMvc(routes =>
            {
                routes
                    .MapRoute(
                        name: "area",
                        template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                    )
                    .MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
