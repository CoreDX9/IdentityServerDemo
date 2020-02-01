using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServerAdmin.Admin.EntityFramework.Shared.DbContexts;
using IdentityServerAdmin.Admin.Helpers;
using IdentityServerAdmin.Admin.Middlewares;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Entity.Identity;

namespace IdentityServerAdmin.Admin.Configuration.Test
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
            services.AddAuthenticationServicesStaging<ApplicationIdentityDbContext, ApplicationUser, ApplicationRole>();
        }

        public override void RegisterAuthorization(IServiceCollection services)
        {
            var rootConfiguration = CreateRootConfiguration();
            services.AddAuthorizationPolicies(rootConfiguration);
        }

        public override void UseAuthentication(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
        }
    }
}






