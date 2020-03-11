using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Configuration;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Configuration.Authorization;
using IdentityServer.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;

namespace IdentityServer.Helpers
{
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
