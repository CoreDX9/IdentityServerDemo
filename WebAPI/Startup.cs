using System;
using System.Collections.Generic;
using System.IO;
using IdentityServer4.AccessTokenValidation;
using Joonasw.AspNetCore.SecurityHeaders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvcCore().AddJsonFormatters();
            services.AddAuthentication((options) =>
                {
                    options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                })
                //.AddJwtBearer(options =>
                //{
                //    options.TokenValidationParameters = new TokenValidationParameters();
                //    options.RequireHttpsMetadata = true;
                //    options.Audience = "api1";//api范围
                //    options.Authority = "https://localhost:5001";//IdentityServer地址
                //})
                //AddIdentityServerAuthentication会在内部调用AddJwtBearer
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.RequireHttpsMetadata = true;
                    options.JwtValidationClockSkew = TimeSpan.FromSeconds(10);
                    options.ApiName = "api1";
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("https://localhost:5007")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddCsp(nonceByteAmount: 32);
            //services.AddHsts(options =>
            //{
            //    options.Preload = true;
            //    options.IncludeSubDomains = true;
            //    options.MaxAge = TimeSpan.FromDays(60);
            //    options.ExcludedHosts.Add("localhost:5000");
            //    options.ExcludedHosts.Add("localhost:5001");
            //    options.ExcludedHosts.Add("localhost:5002");
            //    options.ExcludedHosts.Add("localhost:5003");
            //    options.ExcludedHosts.Add("localhost:5004");
            //    options.ExcludedHosts.Add("localhost:5005");
            //    options.ExcludedHosts.Add("127.0.0.1:5000");
            //    options.ExcludedHosts.Add("127.0.0.1:5001");
            //    options.ExcludedHosts.Add("127.0.0.1:5002");
            //    options.ExcludedHosts.Add("127.0.0.1:5003");
            //    options.ExcludedHosts.Add("127.0.0.1:5004");
            //    options.ExcludedHosts.Add("127.0.0.1:5005");
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "IdentityServer Web API", Version = "v1",
                    Description = "A simple example ASP.NET Core IdentityServer Web API. \r\n IdentityServer clientId: jsIm",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "CoreDX",
                        Email = string.Empty,
                        Url = "http://localhost:5002/Home/Contact"
                    },
                    License = new License
                    {
                        Name = "许可证名字",
                        Url = "http://localhost:5002/Home/Contact"
                    }
                });

                var basePath = Environment.ContentRootPath;

                var xmlPath = Path.Combine(basePath, "WebAPI.xml");

                c.IncludeXmlComments(xmlPath, true);

                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = "https://localhost:5001/connect/authorize",
                    Scopes = new Dictionary<string, string>
                    {
                        { "api1", "api1" },
                    }
                });

                c.OperationFilter<IdentityServer4OAuth2OperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
                //HstsBuilderExtensions.UseHsts(app);
            }

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
                    .From("localhost:5000") //These two domains
                    .From("localhost:5001")
                    .From("localhost:5002")
                    .From("localhost:5003")
                    .From("localhost:5004")
                    .From("localhost:5005")
                    .From("ajax.aspnetcdn.com");
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
                    .From("ajax.aspnetcdn.com");
                //.AddNonce();//此项与AllowUnsafeInline冲突，会被AllowUnsafeInline选项覆盖

                csp.AllowImages
                    .FromSelf()
                    .From("localhost:5000") //These two domains
                    .From("localhost:5001")
                    .From("localhost:5002")
                    .From("localhost:5003")
                    .From("localhost:5004")
                    .From("localhost:5005")
                    .From("ajax.aspnetcdn.com");

                // HTML5 audio and video elemented sources can be from:
                csp.AllowAudioAndVideo
                    .FromNowhere();

                // Contained iframes can be sourced from:
                csp.AllowFrames
                    .FromNowhere(); //Nowhere, no iframes allowed

                // Allow AJAX, WebSocket and EventSource connections to:
                csp.AllowConnections
                    .To("ws://localhost:5000")
                    .To("wss://localhost:5001")
                    .ToSelf();

                // Allow fonts to be downloaded from:
                csp.AllowFonts
                    .FromSelf()
                    .From("ajax.aspnetcdn.com");

                // Allow object, embed, and applet sources from:
                csp.AllowPlugins
                    .FromNowhere();

                // Allow other sites to put this in an iframe?
                csp.AllowFraming
                    .FromNowhere(); // Block framing on other sites, equivalent to X-Frame-Options: DENY

                if (env.IsDevelopment())
                {
                    // Do not block violations, only report
                    // This is a good idea while testing your CSP
                    // Remove it when you know everything will work
                    csp.SetReportOnly();
                    // Where should the violation reports be sent to?
                    csp.ReportViolationsTo("/csp-report");
                }

                // Do not include the CSP header for requests to the /api endpoints
                //csp.OnSendingHeader = context =>
                //{
                //    context.ShouldNotSend = context.HttpContext.Request.Path.StartsWithSegments("/api");
                //    return Task.CompletedTask;
                //};
            });

            app.UseCors("default");

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseMvc();

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI V1");
                });
        }
    }

    /// <summary>
    /// IdentityServer4认证处理
    /// </summary>
    public class IdentityServer4OAuth2OperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {

            if (operation.Security == null)
                operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
            var oAuthRequirements = new Dictionary<string, IEnumerable<string>>
            {

                {"oauth2", new List<string> { "openid", "profile", "api1" }}
            };
            operation.Security.Add(oAuthRequirements);
        }
    }
}
