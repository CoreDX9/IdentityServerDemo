using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Configuration;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Configuration.Authorization;
using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration;
using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration.Constants;
using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration.Interfaces;
using IdentityServer.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;

namespace IdentityServer.Helpers
{
    public static class ConfigureServicesHelper
    {
        /// <summary>
        /// IdentityServer 管理用
        /// </summary>
        /// <returns></returns>
        public static IRootConfiguration CreateRootConfiguration(IConfiguration configuration)
        {
            var rootConfiguration = new RootConfiguration();
            configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).Bind(rootConfiguration.AdminConfiguration);
            configuration.GetSection(ConfigurationConsts.IdentityDataConfigurationKey).Bind(rootConfiguration.IdentityDataConfiguration);
            configuration.GetSection(ConfigurationConsts.IdentityServerDataConfigurationKey).Bind(rootConfiguration.IdentityServerDataConfiguration);
            return rootConfiguration;
        }

        //临时替换服务，内置服务与 DI 中的 AutoMapper 冲突
        public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>(IServiceCollection services)
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

    public class ConfigureSwaggerGenOptions : ConfigureOptionsUseServiceBase<SwaggerGenOptions>
    {
        public ConfigureSwaggerGenOptions(IServiceProvider service) : base(service) { }

        public override void Configure(SwaggerGenOptions options)
        {
            var apiVersionDescription = Service.GetRequiredService<IApiVersionDescriptionProvider>();
            var adminApiConfiguration = Service.GetRequiredService<AdminApiConfiguration>();
            var environment = Service.GetRequiredService<IWebHostEnvironment>();
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
            var xmlPath = Path.Combine(environment.ContentRootPath, xmlFile);
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
        }
    }
}
