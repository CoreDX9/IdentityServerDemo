using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Configuration;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Configuration.Authorization;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.ExceptionHandling;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Mappers;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Resources;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Application.EntityFrameworkCore.IdentityServer;
using CoreDX.Application.EntityFrameworkCore.IdentityServer.Admin;
using CoreDX.Domain.Entity.Identity;
using HealthChecks.UI.Client;
using IdentityServer.Admin.Api.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using System;
using System.Collections.Generic;

namespace IdentityServer.Admin.Api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            HostingEnvironment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var adminApiConfiguration = Configuration.GetSection(nameof(AdminApiConfiguration)).Get<AdminApiConfiguration>();
            services.AddSingleton(adminApiConfiguration);

            // Add DbContexts
            RegisterDbContexts(services, HostingEnvironment);

            services.AddScoped<ControllerExceptionFilterAttribute>();
            services.AddScoped<IApiErrorResources, ApiErrorResources>();

            // Add authentication services
            RegisterAuthentication(services);

            // Add authorization services
            RegisterAuthorization(services);

            var profileTypes = new HashSet<Type>
            {
                typeof(IdentityMapperProfile<RoleDto<int>, int, UserRolesDto<RoleDto<int>, int, int>, int, UserClaimsDto<int>, UserClaimDto<int>, UserProviderDto<int>, UserProvidersDto<int>, UserChangePasswordDto<int>,RoleClaimDto<int>, RoleClaimsDto<int>>)
            };

            services.AddAdminAspNetIdentityServices<ApplicationIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<int>, int, RoleDto<int>, int, int, int,
                ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole,
                ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken,
                UsersDto<UserDto<int>, int>, RolesDto<RoleDto<int>, int>, UserRolesDto<RoleDto<int>, int, int>,
                UserClaimsDto<int>, UserProviderDto<int>, UserProvidersDto<int>, UserChangePasswordDto<int>,
                RoleClaimsDto<int>, UserClaimDto<int>, RoleClaimDto<int>>(profileTypes);

            services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

            services.AddAdminApiCors(adminApiConfiguration);

            services.AddMvcServices<UserDto<int>, int, RoleDto<int>, int, int, int,
                ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole,
                ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken,
                UsersDto<UserDto<int>, int>, RolesDto<RoleDto<int>, int>, UserRolesDto<RoleDto<int>, int, int>,
                UserClaimsDto<int>, UserProviderDto<int>, UserProvidersDto<int>, UserChangePasswordDto<int>,
                RoleClaimsDto<int>>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(adminApiConfiguration.ApiVersion, new OpenApiInfo { Title = adminApiConfiguration.ApiName, Version = adminApiConfiguration.ApiVersion });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{adminApiConfiguration.IdentityServerBaseUrl}/connect/authorize"),
                            Scopes = new Dictionary<string, string> {
                                { adminApiConfiguration.OidcApiName, adminApiConfiguration.ApiName }
                            }
                        }
                    }
                });
                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            services.AddAuditEventLogging<AdminAuditLogDbContext, AuditLog>(Configuration);

            services.AddIdSHealthChecks<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, ApplicationIdentityDbContext, AdminLogDbContext, AdminAuditLogDbContext>(Configuration, adminApiConfiguration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AdminApiConfiguration adminApiConfiguration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{adminApiConfiguration.ApiBaseUrl}/swagger/v1/swagger.json", adminApiConfiguration.ApiName);

                c.OAuthClientId(adminApiConfiguration.OidcSwaggerUIClientId);
                c.OAuthAppName(adminApiConfiguration.ApiName);
            });

            app.UseRouting();
            UseAuthentication(app);
            app.UseCors();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }

        public virtual void RegisterDbContexts(IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            services.AddDbContexts<ApplicationIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext>(Configuration, webHostEnvironment);
        }

        public virtual void RegisterAuthentication(IServiceCollection services)
        {
            var adminApiConfiguration = Configuration.GetSection(nameof(AdminApiConfiguration)).Get<AdminApiConfiguration>();
            services.AddApiAuthentication<ApplicationIdentityDbContext, ApplicationUser, ApplicationRole>(adminApiConfiguration);
        }

        public virtual void RegisterAuthorization(IServiceCollection services)
        {
            services.AddAuthorizationPolicies();
        }

        public virtual void UseAuthentication(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
    }
}






