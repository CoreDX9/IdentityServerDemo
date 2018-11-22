using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Joonasw.AspNetCore.SecurityHeaders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies") //这里的参数要和AddAuthentication(options)的options.DefaultScheme相同
                //.AddOpenIdConnect("oidc", options =>
                //{
                //    options.SignInScheme = "Cookies";

                //    options.Authority = "https://localhost:5001";
                //    options.RequireHttpsMetadata = true;

                //    options.ClientId = "mvcIm";
                //    //options.ClientSecret = "secret";
                //    //options.ResponseType = "code id_token";
                //    options.SaveTokens = true;
                //    //options.GetClaimsFromUserInfoEndpoint = true;

                //    //options.Scope.Add("api1");
                //    //options.Scope.Add("offline_access");
                //})
                .AddOpenIdConnect("oidc", options => //这里的authenticationScheme要和AddAuthentication(options)的options.DefaultChallengeScheme相同
                {
                    //这里的值要和AddCookie()的参数一致
                    //存在多个AddCookie()时和其中一个一致
                    //关联的AddCookie()只需使用最简单的重载即可使用
                    options.SignInScheme = "Cookies";

                    options.Authority = "https://localhost:5001";
                    options.RequireHttpsMetadata = true;

                    options.ClientId = "mvcHcc";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("api1");
                    options.Scope.Add("offline_access");
                });
                //.AddOpenIdConnect("oidc", options =>
                //{
                //    //这里的值要和AddCookie()的参数一致
                //    //存在多个AddCookie()时和其中一个一致
                //    //关联的AddCookie()只需使用最简单的重载即可使用
                //    options.SignInScheme = "Cookies";

                //    options.Authority = "https://demo.identityserver.io/";
                //    options.ClientId = "implicit";
                //    options.SaveTokens = true;

                //    options.ClientSecret = "secret";
                //    options.ResponseType = "code id_token";
                //    options.SaveTokens = true;
                //    options.GetClaimsFromUserInfoEndpoint = true;

                //    options.Scope.Add("api1");
                //    options.Scope.Add("offline_access");
                //});

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
                app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
                //HstsBuilderExtensions.UseHsts(app);
            }

            app.UseHttpsRedirection();

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

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseAuthentication();//要在UseMvc之前

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
