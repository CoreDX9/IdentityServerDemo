using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration;
using CoreDX.Applicaiton.IdnetityServerAdmin.Services;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Application.EntityFrameworkCore.Configuration;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.AuditLogging.EntityFramework.Extensions;
using Skoruba.AuditLogging.EntityFramework.Repositories;
using Skoruba.AuditLogging.EntityFramework.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Helpers;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using System;

namespace IdentityServer.Helpers.IdentityServerAdmin
{
    public static class StartupHelpers
    {
        public static IServiceCollection AddAuditEventLogging<TAuditLoggingDbContext, TAuditLog>(this IServiceCollection services, IConfiguration configuration)
            where TAuditLog : AuditLog, new()
            where TAuditLoggingDbContext : IAuditLoggingDbContext<TAuditLog>
        {
            var auditLoggingConfiguration = configuration.GetSection(nameof(AuditLoggingConfiguration)).Get<AuditLoggingConfiguration>();

            services.AddAuditLogging(options => { options.Source = auditLoggingConfiguration.Source; })
                .AddDefaultHttpEventData(subjectOptions =>
                {
                    subjectOptions.SubjectIdentifierClaim = auditLoggingConfiguration.SubjectIdentifierClaim;
                    subjectOptions.SubjectNameClaim = auditLoggingConfiguration.SubjectNameClaim;
                },
                    actionOptions =>
                    {
                        actionOptions.IncludeFormVariables = auditLoggingConfiguration.IncludeFormVariables;
                    })
                .AddAuditSinks<DatabaseAuditEventLoggerSink<TAuditLog>>();

            // repository for library
            services.AddTransient<IAuditLoggingRepository<TAuditLog>, AuditLoggingRepository<TAuditLoggingDbContext, TAuditLog>>();

            // repository and service for admin
            services.AddTransient<IAuditLogRepository<TAuditLog>, AuditLogRepository<TAuditLoggingDbContext, TAuditLog>>();
            services.AddTransient<IAuditLogService, AuditLogService<TAuditLog>>();

            return services;
        }

        public static void AddIdSHealthChecks<TConfigurationDbContext, TPersistedGrantDbContext, TIdentityDbContext, TLogDbContext, TAuditLoggingDbContext>(this IServiceCollection services, IConfiguration configuration, AdminConfiguration adminConfiguration, string connectionString)
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TIdentityDbContext : DbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TAuditLoggingDbContext : DbContext, IAuditLoggingDbContext<AuditLog>
        {
            var identityServerUri = adminConfiguration.IdentityServerBaseUrl;
            var healthChecksBuilder = services.AddHealthChecks()
                .AddDbContextCheck<TConfigurationDbContext>("ConfigurationDbContext")
                .AddDbContextCheck<TPersistedGrantDbContext>("PersistedGrantsDbContext")
                .AddDbContextCheck<TIdentityDbContext>("IdentityDbContext")
                .AddDbContextCheck<TLogDbContext>("LogDbContext")
                .AddDbContextCheck<TAuditLoggingDbContext>("AuditLogDbContext")
                .AddDbContextCheck<LocalizationModelContext>(nameof(LocalizationModelContext))
                .AddDbContextCheck<ApplicationDbContext>(nameof(ApplicationDbContext))

                .AddIdentityServer(new Uri(identityServerUri), "Identity Server");

            var serviceProvider = services.BuildServiceProvider();
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var configurationTableName = "abc";// DbContextHelpers.GetEntityTable<TConfigurationDbContext>(scope.ServiceProvider);
                var persistedGrantTableName = "abc";// DbContextHelpers.GetEntityTable<TPersistedGrantDbContext>(scope.ServiceProvider);
                var identityTableName = "abc";// DbContextHelpers.GetEntityTable<TIdentityDbContext>(scope.ServiceProvider);
                var logTableName = "abc";// DbContextHelpers.GetEntityTable<TLogDbContext>(scope.ServiceProvider);
                var auditLogTableName = "abc";// DbContextHelpers.GetEntityTable<TAuditLoggingDbContext>(scope.ServiceProvider);
                var LocalizationTableName = "abc";// DbContextHelpers.GetEntityTable<LocalizationModelContext>(scope.ServiceProvider);
                var ApplicationTableName = "abc";// DbContextHelpers.GetEntityTable<ApplicationDbContext>(scope.ServiceProvider);

                var databaseProvider = configuration.GetSection(nameof(DatabaseProviderConfiguration)).Get<DatabaseProviderConfiguration>();
                switch (databaseProvider.ProviderType)
                {
                    case DatabaseProviderType.SqlServer:
                        healthChecksBuilder
                            .AddSqlServer(connectionString, name: "ConfigurationDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{configurationTableName}]")
                            .AddSqlServer(connectionString, name: "PersistentGrantsDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{persistedGrantTableName}]")
                            .AddSqlServer(connectionString, name: "IdentityDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{identityTableName}]")
                            .AddSqlServer(connectionString, name: "LogDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{logTableName}]")
                            .AddSqlServer(connectionString, name: "AuditLogDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{auditLogTableName}]")
                            .AddSqlServer(connectionString, name: "LocalizationModelDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{LocalizationTableName}]")
                            .AddSqlServer(connectionString, name: "ApplicationDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{ApplicationTableName}]");
                        break;
                    case DatabaseProviderType.PostgreSQL:
                        //healthChecksBuilder
                        //    .AddNpgSql(configurationDbConnectionString, name: "ConfigurationDb",
                        //        healthQuery: $"SELECT * FROM {configurationTableName} LIMIT 1")
                        //    .AddNpgSql(persistedGrantsDbConnectionString, name: "PersistentGrantsDb",
                        //        healthQuery: $"SELECT * FROM {persistedGrantTableName} LIMIT 1")
                        //    .AddNpgSql(identityDbConnectionString, name: "IdentityDb",
                        //        healthQuery: $"SELECT * FROM {identityTableName} LIMIT 1")
                        //    .AddNpgSql(logDbConnectionString, name: "LogDb",
                        //        healthQuery: $"SELECT * FROM {logTableName} LIMIT 1")
                        //    .AddNpgSql(auditLogDbConnectionString, name: "AuditLogDb",
                        //        healthQuery: $"SELECT * FROM {auditLogTableName}  LIMIT 1");
                        break;
                    case DatabaseProviderType.MySql:
                        //healthChecksBuilder
                        //    .AddMySql(configurationDbConnectionString, name: "ConfigurationDb")
                        //    .AddMySql(persistedGrantsDbConnectionString, name: "PersistentGrantsDb")
                        //    .AddMySql(identityDbConnectionString, name: "IdentityDb")
                        //    .AddMySql(logDbConnectionString, name: "LogDb")
                        //    .AddMySql(auditLogDbConnectionString, name: "AuditLogDb");
                        break;
                    default:
                        throw new NotImplementedException($"Health checks not defined for database provider {databaseProvider.ProviderType}");
                }
            }
        }
    }
}
