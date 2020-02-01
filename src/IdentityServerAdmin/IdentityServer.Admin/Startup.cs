using System.IdentityModel.Tokens.Jwt;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using IdentityServer.Admin.Configuration.Interfaces;
using IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using IdentityServer.Admin.Helpers;
using IdentityServer.Admin.Configuration;
using IdentityServer.Admin.Configuration.Constants;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Entity.Identity;

namespace IdentityServer.Admin
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            HostingEnvironment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var rootConfiguration = CreateRootConfiguration();
            services.AddSingleton(rootConfiguration);

            // Add DbContexts for Asp.Net Core Identity, Logging and IdentityServer - Configuration store and Operational store
            RegisterDbContexts(services, HostingEnvironment);

            // Add Asp.Net Core Identity Configuration and OpenIdConnect auth as well
            RegisterAuthentication(services);
            
            // Add exception filters in MVC
            services.AddMvcExceptionFilters();

            // Add all dependencies for IdentityServer Admin
            services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

            // Add all dependencies for Asp.Net Core Identity
            // If you want to change primary keys or use another db model for Asp.Net Core Identity:
            services.AddAdminAspNetIdentityServices<ApplicationIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<int>, int, RoleDto<int>, int, int, int,
                                ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole,
                                ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken,
                                UsersDto<UserDto<int>, int>, RolesDto<RoleDto<int>, int>, UserRolesDto<RoleDto<int>, int, int>,
                                UserClaimsDto<int>, UserProviderDto<int>, UserProvidersDto<int>, UserChangePasswordDto<int>,
                                RoleClaimsDto<int>, UserClaimDto<int>, RoleClaimDto<int>>();
            
            // Add all dependencies for Asp.Net Core Identity in MVC - these dependencies are injected into generic Controllers
            // Including settings for MVC and Localization
            // If you want to change primary keys or use another db model for Asp.Net Core Identity:
            services.AddMvcWithLocalization<UserDto<int>, int, RoleDto<int>, int, int, int,
                ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole,
                ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken,
                UsersDto<UserDto<int>, int>, RolesDto<RoleDto<int>, int>, UserRolesDto<RoleDto<int>, int, int>,
                UserClaimsDto<int>, UserProviderDto<int>, UserProvidersDto<int>, UserChangePasswordDto<int>,
                RoleClaimsDto<int>>(Configuration);

            // Add authorization policies for MVC
            RegisterAuthorization(services);

            // Add audit logging
            services.AddAuditEventLogging<AdminAuditLogDbContext, AuditLog>(Configuration);

            services.AddIdSHealthChecks<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, ApplicationIdentityDbContext, AdminLogDbContext, AdminAuditLogDbContext>(Configuration, rootConfiguration.AdminConfiguration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Add custom security headers
            app.UseSecurityHeaders();

            app.UseStaticFiles();

            UseAuthentication(app);

            // Use Localization
            app.ConfigureLocalization();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoint => 
            { 
                endpoint.MapDefaultControllerRoute();
                endpoint.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });                
            });
        }

        public virtual void RegisterDbContexts(IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            services.RegisterDbContexts<ApplicationIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext>(Configuration, webHostEnvironment);
        }

        public virtual void RegisterAuthentication(IServiceCollection services)
        {
            var rootConfiguration = CreateRootConfiguration();
            services.AddAuthenticationServices<ApplicationIdentityDbContext, ApplicationUser, ApplicationRole>(rootConfiguration.AdminConfiguration);
        }

        public virtual void RegisterAuthorization(IServiceCollection services)
        {
            var rootConfiguration = CreateRootConfiguration();
            services.AddAuthorizationPolicies(rootConfiguration);
        }

        public virtual void UseAuthentication(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }

        protected IRootConfiguration CreateRootConfiguration()
        {
            var rootConfiguration = new RootConfiguration();
            Configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).Bind(rootConfiguration.AdminConfiguration);
            Configuration.GetSection(ConfigurationConsts.IdentityDataConfigurationKey).Bind(rootConfiguration.IdentityDataConfiguration);
            Configuration.GetSection(ConfigurationConsts.IdentityServerDataConfigurationKey).Bind(rootConfiguration.IdentityServerDataConfiguration);
            return rootConfiguration;
        }
    }
}





