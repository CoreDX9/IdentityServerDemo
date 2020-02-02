using IdentityServer.CustomServices;
using Localization.SqlLocalizer;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using System;

namespace IdentityServer.Extensions
{
    public static class MixedLocalizationServiceCollectionExtensions
    {
        public static IServiceCollection AddMixedLocalization(
            this IServiceCollection services,
            Action<LocalizationOptions> setupBuiltInAction = null,
            Action<SqlLocalizationOptions> setupSqlAction = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IMiscibleStringLocalizerFactory, MiscibleResourceManagerStringLocalizerFactory>();

            services.AddSingleton<IMiscibleStringLocalizerFactory, MiscibleSqlStringLocalizerFactory>();
            services.TryAddSingleton<IStringExtendedLocalizerFactory, MiscibleSqlStringLocalizerFactory>();
            services.TryAddSingleton<DevelopmentSetup>();

            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

            services.AddSingleton<IStringLocalizerFactory, MixedStringLocalizerFactory>();

            if (setupBuiltInAction != null) services.Configure(setupBuiltInAction);
            if (setupSqlAction != null) services.Configure(setupSqlAction);

            return services;
        }
    }
}
