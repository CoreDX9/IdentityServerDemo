using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Entity.Identity;
using IdentityServer4.AccessTokenValidation;
using IdentityServer.Admin.Api.Helpers;
using IdentityServer.Admin.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CoreDX.Application.EntityFrameworkCore.IdentityServer;
using CoreDX.Application.EntityFrameworkCore.IdentityServer.Admin;

namespace IdentityServer.Admin.Api.Configuration.Test
{
    public class StartupTest : Startup
    {
        public StartupTest(IWebHostEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        public override void RegisterDbContexts(IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            services.RegisterDbContextsStaging<ApplicationIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext>();
        }

        public override void RegisterAuthentication(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options => { options.User.RequireUniqueEmail = true; })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(IdentityServerAuthenticationDefaults.AuthenticationScheme);
        }

        public override void RegisterAuthorization(IServiceCollection services)
        {
            services.AddAuthorizationPolicies();
        }

        public override void UseAuthentication(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
        }
    }
}





