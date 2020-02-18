#region using

using AspNetCoreRateLimit;
using AutoMapper;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Core.Command;
using CoreDX.Domain.Core.Event;
using CoreDX.Domain.Entity.Identity;
using CoreDX.Domain.Model.Command;
using CoreDX.Domain.Model.Event;
using CoreDX.Domain.Repository.EntityFrameworkCore;
using CoreDX.Identity.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using IdentityServer.CustomMiddlewares;
using IdentityServer.CustomServices;
using IdentityServer.Extensions;
using IdentityServer.Grpc.Services;
using IdentityServer.Hubs;
using IdentityServer4.Configuration;
using Joonasw.AspNetCore.SecurityHeaders;
using Localization.SqlLocalizer.DbStringLocalizer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Linq;
using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration.Interfaces;
using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration;
using CoreDX.Applicaiton.IdnetityServerAdmin.MvcFilters;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CoreDX.Applicaiton.IdnetityServerAdmin.Helpers.Localization;
using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration.Constants;
using IdentityServer.Helpers.IdentityServerAdmin;
using Skoruba.AuditLogging.EntityFramework.Entities;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore.IdentityServer;
using CoreDX.Application.EntityFrameworkCore.IdentityServer.Admin;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Configuration.Authorization;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Configuration;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using CoreDX.Applicaiton.IdnetityServerAdmin.Services;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories;
using Microsoft.AspNetCore.ResponseCompression;

#endregion

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // 异步控制器动作或Razor页面动作返回void可能导致从DI容器获取的efcore或者其他对象被释放，返回Task可以避免这个问题
        public void ConfigureServices(IServiceCollection services)
        {
            #region 注册 IdentityServer 管理用配置项

            var rootConfiguration = CreateRootConfiguration();
            services.AddSingleton(rootConfiguration);

            var adminApiConfiguration = Configuration.GetSection(nameof(AdminApiConfiguration)).Get<AdminApiConfiguration>();
            services.AddSingleton(adminApiConfiguration);

            #endregion

            #region 数据库连接配置

            var useInMemoryDatabase = Configuration.GetValue("UseInMemoryDatabase", false);

            //内存数据库配置
            InMemoryDatabaseRoot inMemoryDatabaseRoot = useInMemoryDatabase ? new InMemoryDatabaseRoot() : null;

            //真实数据库配置
            var connectionString = useInMemoryDatabase ? string.Empty : Configuration.GetConnectionString("IdentityServerDbContextConnection")
                .Replace("{EnvironmentName}", Environment.EnvironmentName);

            //重定向数据库文件（默认文件在用户文件夹，改到项目内部文件夹方便管理）
            if (!useInMemoryDatabase)
            {
                connectionString += $@";AttachDbFileName={Environment.ContentRootPath}\App_Data\Database\IdentityServerDb-{Environment.EnvironmentName}.mdf";
            }

            //迁移程序集名
            var migrationsAssemblyName = useInMemoryDatabase
                ? string.Empty
                : typeof(CoreDX.Application.DbMigration.Application.InitialApplicationDbMigration).Assembly.GetName().Name;

            #endregion

            //注册相应压缩服务
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            //注册Http上下文访问服务
            services.AddHttpContextAccessor();

            //注册内存缓存
            services.AddMemoryCache();

            //注册应用数据保护服务
            services.AddDataProtection()
                .SetApplicationName("IdentityServerDemo")
                .PersistKeysToFileSystem(new DirectoryInfo($@"{Environment.ContentRootPath}\App_Data\DataProtectionKey"));

            //注册响应压缩服务（gzip）
            services.AddResponseCompression();

            //注册网站健康检查服务
            services.AddHealthChecks();

            //注册文件夹浏览服务
            services.AddDirectoryBrowser();

            //注册 MiniProfiler 服务
            services.AddMiniProfiler(options =>
            {
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomRight;
                options.PopupShowTimeWithChildren = true;
                options.RouteBasePath = "/MiniProfilerBase";

                // 请确保已经在迁移中创建了表，本演示已经在初始迁移中集成了表创建
                if(Configuration.GetValue("SaveMiniProfilerData", false))
                {
                    options.Storage = new StackExchange.Profiling.Storage.SqlServerStorage(connectionString);
                }
            }).AddEntityFramework();

            #region 注册 AutoMapper 服务

            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            AutoMapper.IConfigurationProvider config = new MapperConfiguration(cfg =>
            {
                var profileTypes =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsSubclassOf(typeof(Profile)) && !type.IsGenericType && !type.IsAbstract
                select type;

                var profiles = profileTypes.Select(x =>
                    {
                        try
                        {
                            return (Profile)Activator.CreateInstance(x);
                        }
                        catch (MissingMethodException ex)
                        {
                            return null;
                        }
                        catch(Exception ex)
                        {
                            throw new Exception("实例化映射配置失败。", ex);
                        }
                    }).Where(x => x != null);
                cfg.AddProfiles(profiles);
            });
            services.AddSingleton(config);
            services.AddScoped<IMapper, Mapper>();

            #endregion

            #region 注册 Identity EF上下文

            //注册EF上下文
            if (useInMemoryDatabase)
            {
                services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<ApplicationIdentityDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("IdentityServerDb-InMemory", inMemoryDatabaseRoot);
                    })
                    .AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("IdentityServerDb-InMemory", inMemoryDatabaseRoot);
                    })
                    .AddDbContext<ApplicationPermissionDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("IdentityServerDb-InMemory", inMemoryDatabaseRoot);
                    });
            }
            else
            {
                services.AddDbContext<ApplicationIdentityDbContext>(options =>
                {
                    options.UseSqlServer(connectionString, b =>
                    {
                        b.MigrationsAssembly(migrationsAssemblyName);
                        b.EnableRetryOnFailure(3);
                    });
                });
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(connectionString, b =>
                    {
                        b.MigrationsAssembly(migrationsAssemblyName);
                        b.EnableRetryOnFailure(3);
                    });
                });
                services.AddDbContext<ApplicationPermissionDbContext>(options =>
                {
                    options.UseSqlServer(connectionString, b =>
                    {
                        b.MigrationsAssembly(migrationsAssemblyName);
                        b.EnableRetryOnFailure(3);
                    });
                });
            }

            #endregion

            #region 注册 Identity 服务

            services.AddSingleton(new ProtectorOptions { KeyPath = $@"{Environment.ContentRootPath}\App_Data\AesDataProtectionKey" });

            //注册Identity服务（使用EF存储，在EF上下文之后注册）
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
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                //注册Identity隐私数据保护服务（启用隐私数据保护后必须注册，和上面那个应用数据保护服务不一样，这个是给IdentityDbContext用的，上面那个是给cookies之类加密用的）
                .AddPersonalDataProtection<AesProtector, AesProtectorKeyRing>()
                .AddDefaultTokenProviders();

            #endregion

            #region 注册 IdentityServer4 服务

            //结合EFCore生成IdentityServer4数据库迁移命令详情见 CoreDX.Application.EntityFrameworkCore 项目说明文档
            //添加IdentityServer4对EFCore数据库的支持
            //但是这里需要初始化数据 默认生成的数据库中是没有配置数据

            //注册IdentityServer4服务
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

                //IdentityServer支持一些端点的CORS。底层CORS实现是从ASP.NET Core提供的，因此它会自动注册在依赖注册系统中
                //options.Cors = new CorsOptions
                //{
                //    CorsPaths = { "/" }, //支持CORS的IdentityServer中的端点。默认为发现，用户信息，令牌和撤销终结点
                //    CorsPolicyName =
                //        "default", //【必备】将CORS请求评估为IdentityServer的CORS策略的名称（默认为"IdentityServer4"）。处理这个问题的策略提供者是ICorsPolicyService在依赖注册系统中注册的。如果您想定制允许连接的一组CORS原点，则建议您提供一个自定义的实现ICorsPolicyService
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
                id4.AddConfigurationStore<IdentityServerConfigurationDbContext>(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseInMemoryDatabase("IdentityServerDb-InMemory", inMemoryDatabaseRoot);
                })
                    .AddOperationalStore<IdentityServerPersistedGrantDbContext>(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseInMemoryDatabase("IdentityServerDb-InMemory", inMemoryDatabaseRoot);

                        options.EnableTokenCleanup = true;
                    });
            }
            else
            {
                // this adds the config data from DB (clients, resources)
                id4.AddConfigurationStore<IdentityServerConfigurationDbContext>(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssemblyName));
                })
                    // this adds the operational data from DB (codes, tokens, consents)
                    .AddOperationalStore<IdentityServerPersistedGrantDbContext>(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseSqlServer(connectionString,
                                sql => sql.MigrationsAssembly(migrationsAssemblyName));

                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                        // options.TokenCleanupInterval = 15; // frequency in seconds to cleanup stale grants. 15 is useful during debugging
                    });
            }

            #endregion

            #region 注册 IdentityServer4 管理需要的上下文

            if (useInMemoryDatabase)
            {
                services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<AdminLogDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("IdentityServerDb-InMemory", inMemoryDatabaseRoot);
                    })
                    .AddDbContext<AdminAuditLogDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("IdentityServerDb-InMemory", inMemoryDatabaseRoot);
                    });
            }
            else
            {
                // Log DB from existing connection
                services.AddDbContext<AdminLogDbContext>(options =>
                {
                    options.UseSqlServer(connectionString, optionsSql =>
                    {
                        optionsSql.MigrationsAssembly(migrationsAssemblyName);
                        optionsSql.EnableRetryOnFailure(3);
                    });
                });

                // Audit logging connection
                services.AddDbContext<AdminAuditLogDbContext>(options =>
                {
                    options.UseSqlServer(connectionString, optionsSql =>
                    {
                        optionsSql.MigrationsAssembly(migrationsAssemblyName);
                        optionsSql.EnableRetryOnFailure(3);
                    });
                });
            }

            #endregion

            #region 注册管理 IdentityServer4 相关的服务

            services.AddScoped<ControllerExceptionFilterAttribute>();
            services.AddScoped<CoreDX.Applicaiton.IdnetityServerAdmin.Api.Resources.IApiErrorResources, CoreDX.Applicaiton.IdnetityServerAdmin.Api.Resources.ApiErrorResources>();

            //内置服务与 DI 中的 AutoMapper 冲突
            //services.AddAdminServices<IdentityServerConfigurationDbContext,
            //    IdentityServerPersistedGrantDbContext, AdminLogDbContext>();
            services.AddTransient<IClientRepository, ClientRepository<IdentityServerConfigurationDbContext>>();
            services.AddTransient<IIdentityResourceRepository, IdentityResourceRepository<IdentityServerConfigurationDbContext>>();
            services.AddTransient<IApiResourceRepository, ApiResourceRepository<IdentityServerConfigurationDbContext>>();
            services.AddTransient<IPersistedGrantRepository, PersistedGrantRepository<IdentityServerPersistedGrantDbContext>>();
            services.AddTransient<ILogRepository, LogRepository<AdminLogDbContext>>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IApiResourceService, ApiResourceService>();
            services.AddTransient<IIdentityResourceService, IdentityResourceService>();
            services.AddTransient<IPersistedGrantService, PersistedGrantService>();
            services.AddTransient<ILogService, Skoruba.IdentityServer4.Admin.BusinessLogic.Services.LogService>();
            services.AddScoped<IApiResourceServiceResources, ApiResourceServiceResources>();
            services.AddScoped<IClientServiceResources, ClientServiceResources>();
            services.AddScoped<IIdentityResourceServiceResources, IdentityResourceServiceResources>();
            services.AddScoped<IPersistedGrantServiceResources, PersistedGrantServiceResources>();

            //内置服务与 DI 中的 AutoMapper 冲突
            //services.AddAdminAspNetIdentityServices<ApplicationIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<int>, int, RoleDto<int>, int, int, int,
            //        ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole,
            //        ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken,
            //        UsersDto<UserDto<int>, int>, RolesDto<RoleDto<int>, int>, UserRolesDto<RoleDto<int>, int, int>,
            //        UserClaimsDto<int>, UserProviderDto<int>, UserProvidersDto<int>, UserChangePasswordDto<int>,
            //        RoleClaimsDto<int>, UserClaimDto<int>, RoleClaimDto<int>>();
            AddAdminAspNetIdentityServices<ApplicationIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<int>, int, RoleDto<int>, int, int, int,
                    ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole,
                    ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken,
                    UsersDto<UserDto<int>, int>, RolesDto<RoleDto<int>, int>, UserRolesDto<RoleDto<int>, int, int>,
                    UserClaimsDto<int>, UserProviderDto<int>, UserProvidersDto<int>, UserChangePasswordDto<int>,
                    RoleClaimsDto<int>, UserClaimDto<int>, RoleClaimDto<int>>(services);

            // Add audit logging
            services.AddAuditEventLogging<AdminAuditLogDbContext, AuditLog>(Configuration);

            services.AddIdSHealthChecks<IdentityServerConfigurationDbContext,
                IdentityServerPersistedGrantDbContext, ApplicationIdentityDbContext, AdminLogDbContext,
                AdminAuditLogDbContext>(Configuration, rootConfiguration.AdminConfiguration, connectionString);
            #endregion

            #region 注册 FluentValidation 服务

            //注册FluentValidation验证器
            services.AddTransient<IValidator<Pages.FluentValidationDemo.IndexModel.A>, Pages.FluentValidationDemo.IndexModel.AValidator>();

            //AssemblyScanner.FindValidatorsInAssemblyContaining<Startup>().ForEach(pair => {
            //    // RegisterValidatorsFromAssemblyContaing does this:
            //    services.Add(ServiceDescriptor.Transient(pair.InterfaceType, pair.ValidatorType));
            //    // Also register it as its concrete type as well as the interface type
            //    services.Add(ServiceDescriptor.Transient(pair.ValidatorType, pair.ValidatorType));
            //});

            #endregion

            #region 注册处理访问请求功能的相关服务

            //注册MVC相关服务
            services.AddMvc(options =>//在这里添加的过滤器可以使用构造方法依赖注册获取任何已经注册到服务容器的服务
                {
                    //options.Filters.Add<MyAsyncPageFilter>();
                    //options.Filters.Add<MyAuthorizeAttribute>();
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());//自动对控制器的post，put，delete，patch请求进行保护
                    options.Conventions.Add(new GenericControllerRouteConvention());//给 IdentityServer 管理用的
                })
                //使用NewtonsoftJson替换微软内置的Json框架（Core 3.x开始）
                .AddNewtonsoftJson()
                //启用Razor视图的动态编译
                .AddRazorRuntimeCompilation()
                //注册FluentValidation服务
                .AddFluentValidation(fv =>
                {
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
                    fv.ImplicitlyValidateChildProperties = true;
                })
                //注册视图本地化服务
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; })
                //注册数据注解本地化服务
                .AddDataAnnotationsLocalization()
                .ConfigureApplicationPartManager(m =>
                {
                    m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider<UserDto<int>, int, RoleDto<int>, int, int, int,
                        ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole,
                        ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken,
                        UsersDto<UserDto<int>, int>, RolesDto<RoleDto<int>, int>, UserRolesDto<RoleDto<int>, int, int>,
                        UserClaimsDto<int>, UserProviderDto<int>, UserProvidersDto<int>, UserChangePasswordDto<int>,
                        RoleClaimsDto<int>>());
                })
                //设定兼容性
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #region 注册SignalR服务

            //注册SignalR服务
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

            #endregion

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader(), new HeaderApiVersionReader() { HeaderNames = { "x-api-version" } });
            })
                .AddVersionedApiExplorer(option =>
            {
                option.GroupNameFormat = "'v'VVVV";//api组名格式
                option.AssumeDefaultVersionWhenUnspecified = true;//是否提供API版本服务
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
                using var rootProvider = services.BuildServiceProvider();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
                using var service = rootProvider.CreateScope();

                var apiVersionDescription = service.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in apiVersionDescription.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName,
                         new OpenApiInfo()
                         {
                             Title = $"My API {description.ApiVersion}",
                             Version = description.ApiVersion.ToString(),
                             Description = $"A simple example ASP.NET Core Web API  \r\n IdentityServerDemo clientId: jsIm, scopes: api1.  \r\n IdentityServer4Admin clientId: {adminApiConfiguration.OidcSwaggerUIClientId}, scopes: {adminApiConfiguration.ApiName}.",
                             TermsOfService = new Uri("https://example.com/coredx"),
                             Contact = new OpenApiContact
                             {
                                 Name = "CoreDX",
                                 Email = string.Empty,
                                 Url = new Uri("https://example.com/coredx"),
                             },
                             License = new OpenApiLicense
                             {
                                 Name = "Use under LICX",
                                 Url = new Uri("https://example.com/license"),
                             }
                         }
                    );
                }

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{typeof(Startup).Assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(Environment.ContentRootPath, xmlFile);
                options.IncludeXmlComments(xmlPath);

                var authorizationUrl = string.Empty;
#if DEBUG
                authorizationUrl = "https://localhost:5001/connect/authorize";
#else
                authorizationUrl = "https://localhost/connect/authorize";
#endif

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri(authorizationUrl),
                            Scopes = new Dictionary<string, string> {
                                { "api1", "api1" },
                                { adminApiConfiguration.OidcApiName, adminApiConfiguration.ApiName }
                            }
                        },
                    },
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        new[] { "api1" }
                    }
                });

                //options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            //注册Grpc服务
            services.AddGrpc();

            #endregion

            #region 注册本地化服务，配置本地化数据源（要在注册 MVC 时配置启用本地化才有效）

            //注册基于 EFCore 的本地化数据服务存储服务
            if (useInMemoryDatabase)
            {
                services.AddDbContext<LocalizationModelContext>(
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

            //注册混合本地化服务工厂，先使用基于 ResourceManager 的本地化字符串，如果没有找到，再使用基于 EFCore 存储的本地化字符串
            //（可配置为在没有找到相应记录时自动创建记录，需要先注册LocalizationModelContext）
            services.AddMixedLocalization(opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; },
                options => options.UseSettings(true, false, true, true));

            //这个是给 IdentityServer 管理用的
            services.TryAddTransient(typeof(IGenericControllerLocalizer<>), typeof(GenericControllerLocalizer<>));

            //配置请求本地化选项
            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var cultures =  Configuration.GetSection("Internationalization").GetSection("Cultures")
                    .Get<List<string>>()
                    .Select(x => new CultureInfo(x)).ToList();
                    var supportedCultures = cultures;

                    var defaultRequestCulture = cultures.FirstOrDefault() ?? new CultureInfo("zh-CN");
                    options.DefaultRequestCulture = new RequestCulture(culture: defaultRequestCulture, uiCulture: defaultRequestCulture);
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });

            #endregion

            #region 注册身份验证服务

            //注册身份验证服务
            services.AddAuthentication()
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
            //安装以下包后下面这段代码可用
            //< PackageReference Include = "IdentityServer4.AccessTokenValidation" Version = "x.x.x" />
            //.AddIdentityServerAuthentication(options =>
            //{
            //    options.Authority = "https://localhost:5001";
            //    options.RequireHttpsMetadata = true;
            //    options.JwtValidationClockSkew = TimeSpan.FromSeconds(10);
            //    options.ApiName = "api1";
            //});

            #endregion

            #region 注册配置安全相关服务

            services.AddAuthorization(options =>
            {
                //IdentityServer 管理用
                options.AddPolicy(AuthorizationConsts.AdministrationPolicy,
                    policy => policy.RequireRole(rootConfiguration.AdminConfiguration.AdministrationRole));
            });

            //注册跨域访问服务
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.WithOrigins("https://localhost:5001", "https://localhost:5003", "https://localhost:5005", "https://localhost:5007")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }));

            //注册反CSRF服务并配置请求头名称
            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

            //注册CSP（内容安全策略）服务
            services.AddCsp();

            //注册Hsts（传输安全）服务
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

            #endregion

            #region 注册配置访问限流服务

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            //load general configuration from appsettings.json
            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            //load client rules from appsettings.json
            services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));

            // 注册限流数据内存存储服务，依赖内存缓存服务
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            // 注册限流数据分布式存储服务，依赖Redis
            //services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            //services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
            //services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            #endregion

            #region 注册自己写的各种服务

            #region 配置 RabbitMQ 使用测试

            //在没有RabbitMQ的机器上运行项目设置false，现在暂时没什么用
            //if (Configuration.GetValue("UseEntityHistory", false))
            //{
            //    //注册实体历史记录服务，供ApplicationIdentityDbContext用
            //    //services.AddEntityHistoryRecorder(Configuration);
            //}

            #endregion

            #region DDD+CQRS+EDA 相关服务

            services.AddScoped(typeof(ICommandBus<>), typeof(MediatRCommandBus<>));
            services.AddScoped(typeof(ICommandBus<,>), typeof(MediatRCommandBus<,>));
            services.AddScoped(typeof(ICommandStore), typeof(InProcessCommandStore));
            services.AddScoped(typeof(IEventBus), typeof(MediatREventBus));
            services.AddScoped(typeof(IEventBus<>), typeof(MediatREventBus<>));
            services.AddScoped(typeof(IEventStore), typeof(InProcessEventStore));
            services.AddScoped(typeof(IEFCoreRepository<,>), typeof(EFCoreRepository<,>));
            services.AddScoped(typeof(IEFCoreRepository<,,>), typeof(EFCoreRepository<,,>));
            services.AddMediatR(typeof(CoreDX.Application.Command.UserManage.ListUserCommandHandler).GetTypeInfo().Assembly);

            #endregion

            //注册服务配置表
            services.AddSingleton(services);

            //注册请求处理器信息获取服务
            services.AddSingleton<IRequestHandlerInfo, RequestHandlerInfo>();

            //注册视图渲染服务
            services.AddTransient<RazorViewToStringRenderer>();

            //注册电子邮件发送服务（实际是在桌面生成一个网页文件）
            services.AddScoped<IEmailSender, EmailSender>();

            //注册（工厂方式激活的）自定义中间件服务
            services.AddScoped<AntiforgeryTokenGenerateMiddleware>();

            //注册无界面chrome服务
            services.AddSingleton<HeadlessChromeManager>();

            //注册直播服务主机管理器
            services.AddSingleton<RtmpServerManager>();

            #endregion

            #region 注册应用 cookie 配置，一定要放在最后
            //这个单例配置不知道为什么被注册了快20次，不放在最后很可能被中途不知道哪个服务注册扩展给你覆盖掉
            //导致自定义配置无法执行，坑爹

            // 注册 cookie 配置
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                //GDPR支持配置
                options.CheckConsentNeeded = context => true;
                //options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //配置Identity跳转链接（对于 Api 调用，直接返回响应状态码，不做跳转）
            services.ConfigureApplicationCookie(options =>
            {
                var onRedirectToLogin = options.Events.OnRedirectToLogin;
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return System.Threading.Tasks.Task.CompletedTask;
                    }

                    return onRedirectToLogin(context);
                };

                var onRedirectToAccessDenied = options.Events.OnRedirectToAccessDenied;
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    if (context.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return System.Threading.Tasks.Task.CompletedTask;
                    }

                    return onRedirectToAccessDenied(context);
                };

                options.LoginPath = new PathString("/IdentityServer/Account/Login");
                options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //注册管道是有顺序的，先注册的中间在请求处理管道中会先运行
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescription, AdminApiConfiguration adminApiConfiguration)
        {
            //配置FluentValidation验证信息的本地化，这个不是中间件，只是需要 IApplicationBuilder 提供参数，所以放这里
            app.ConfigLocalizationFluentValidation();

            //注册响应压缩到管道
            app.UseResponseCompression();

            //注册请求限流到管道
            //app.UseIpRateLimiting();
            //app.UseClientRateLimiting();

            if (env.IsDevelopment())
            {
                //注册开发环境异常信息页面
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBlazorDebugging();
            }
            else
            {
                //注册异常错误页面
                app.UseExceptionHandler("/Home/Error");
            }

            //捕获异常用（不然开发环境异常信息页会导致vs不中断，不好调试）
            //app.UseExMiddleware();

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
                    .From("blob:")
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
                    .FromSelf()
                    .From("blob:");

                // Contained iframes can be sourced from:
                csp.AllowFrames
                    .FromSelf();

                // Allow AJAX, WebSocket and EventSource connections to:
                csp.AllowConnections
                    .ToSelf()
                    .To("ws://localhost:8080")
                    .To("ws://localhost:5000")
                    .To("wss://localhost:5001");

                // Allow fonts to be downloaded from:
                csp.AllowFonts
                    .FromSelf()
                    .From("data:")
                    .From("fonts.gstatic.com")
                    .From("ajax.aspnetcdn.com");

                // Allow object, embed, and applet sources from:
                csp.AllowPlugins
                    .FromSelf();

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

            #region 注册网站文件夹浏览

            //注册文件浏览器
            var dir = new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(Environment.ContentRootPath),
                RequestPath = "/dir"
            };
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

            #endregion

            //注册 Blazor 客户端文件
            app.UseClientSideBlazorFiles<BlazorApp.Client.Program>();

            #region 注册 Swagger

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescription.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }

                options.OAuthClientId(adminApiConfiguration.OidcSwaggerUIClientId);
                options.OAuthAppName(adminApiConfiguration.ApiName);
            });

            #endregion

            //注册开发环境的npm资源
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
            }

            //注册静态文件到管道（wwwroot文件夹）
            app.UseStaticFiles();

            //注册 MiniProfiler 到管道
            app.UseMiniProfiler();

            //注册路由到管道
            app.UseRouting();

            //注册Cookie策略到管道（GDPR）
            app.UseCookiePolicy();

            //注册跨域策略到管道
            app.UseCors("CorsPolicy");

            //注册IdentityServer4到管道
            app.UseIdentityServer();
            //新版IdentityServer4要自己调用；
            app.UseAuthorization();

            //注册自定义中间件到管道
            app.UseAntiforgeryTokenGenerateMiddleware();

            //注册终结点到管道（SignalR集线器和Mvc路由集中在这里配置）
            //为天堂的Mvc中间件默哀3秒，被终结点中间件上位，彻底沦为一堆服务 ( ╯□╰ )
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/.well-known/acme-challenge/oeqphuvkAh-nkRhOmfgwK0jin33MZFvdY84t96Dei88", context =>
                    context.Response.WriteAsync("oeqphuvkAh-nkRhOmfgwK0jin33MZFvdY84t96Dei88.MH3xs1jYDVTn05jfUtv4-utqDcgZnd4adWIrVgwTujg"));

                //映射 SignalR 集线器终结点
                endpoints.MapHub<ChatHub>("/chatHub");

                //映射健康检查终结点
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                //临时修复 IdentityServer 管理的登录跳转链接错误
                endpoints.MapGet("/Account/Login", context =>
                    {
                        context.Response.Redirect($"/Identity{context.Request.Path}{context.Request.QueryString}");
                        return Task.CompletedTask;
                    });

                //映射区域控制器终结点
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                //映射默认控制终结点
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                //映射 Razor Pages 终结点
                endpoints.MapRazorPages();

                //映射gRPC终结点
                endpoints.MapGrpcService<GreeterService>();

                //映射 Blazor 客户端终结点
                endpoints.MapFallbackToClientSideBlazor<BlazorApp.Client.Program>("/blazor/{**subPath}", "index.html");
            });
        }

        /// <summary>
        /// IdentityServer 管理用
        /// </summary>
        /// <returns></returns>
        private IRootConfiguration CreateRootConfiguration()
        {
            var rootConfiguration = new RootConfiguration();
            Configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).Bind(rootConfiguration.AdminConfiguration);
            Configuration.GetSection(ConfigurationConsts.IdentityDataConfigurationKey).Bind(rootConfiguration.IdentityDataConfiguration);
            Configuration.GetSection(ConfigurationConsts.IdentityServerDataConfigurationKey).Bind(rootConfiguration.IdentityServerDataConfiguration);
            return rootConfiguration;
        }

        //临时替换服务，内置服务与 DI 中的 AutoMapper 冲突
        private static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>(IServiceCollection services)
            where TIdentityDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
            where TPersistedGrantDbContext : DbContext, Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces.IAdminPersistedGrantDbContext
            where TUserDto : UserDto<TUserDtoKey>
            where TRoleDto : RoleDto<TRoleDtoKey>
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TUsersDto : UsersDto<TUserDto, TUserDtoKey>
            where TRolesDto : RolesDto<TRoleDto, TRoleDtoKey>
            where TUserRolesDto : UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>
            where TUserClaimsDto : UserClaimsDto<TUserDtoKey>
            where TUserProviderDto : UserProviderDto<TUserDtoKey>
            where TUserProvidersDto : UserProvidersDto<TUserDtoKey>
            where TUserChangePasswordDto : UserChangePasswordDto<TUserDtoKey>
            where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>
            where TUserClaimDto : UserClaimDto<TUserDtoKey>
            where TRoleClaimDto : RoleClaimDto<TRoleDtoKey>
        {
            services.AddTransient<IIdentityRepository<TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IdentityRepository<TIdentityDbContext, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
            services.AddTransient<IPersistedGrantAspNetIdentityRepository, PersistedGrantAspNetIdentityRepository<TIdentityDbContext, TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
            services.AddTransient<Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces.IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>, CoreDX.Applicaiton.IdnetityServerAdmin.Services.Identity.IdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>>();
            services.AddTransient<Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces.IPersistedGrantAspNetIdentityService, CoreDX.Applicaiton.IdnetityServerAdmin.Services.Identity.PersistedGrantAspNetIdentityService>();
            services.AddScoped<Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources.IIdentityServiceResources, Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources.IdentityServiceResources>();
            services.AddScoped<Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources.IPersistedGrantAspNetIdentityServiceResources, Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources.PersistedGrantAspNetIdentityServiceResources>();
            return services;
        }
    }
}
