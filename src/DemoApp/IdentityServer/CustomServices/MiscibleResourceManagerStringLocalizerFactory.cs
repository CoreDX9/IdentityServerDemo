using Localization.SqlLocalizer;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IdentityServer.CustomServices
{
    public interface IMiscibleStringLocalizerFactory : IStringLocalizerFactory
    {
    }

    public class MiscibleResourceManagerStringLocalizerFactory : ResourceManagerStringLocalizerFactory, IMiscibleStringLocalizerFactory
    {
        public MiscibleResourceManagerStringLocalizerFactory(IOptions<LocalizationOptions> localizationOptions, ILoggerFactory loggerFactory) : base(localizationOptions, loggerFactory)
        {
        }
    }

    public class MiscibleSqlStringLocalizerFactory : SqlStringLocalizerFactory, IStringExtendedLocalizerFactory, IMiscibleStringLocalizerFactory
    {
        public MiscibleSqlStringLocalizerFactory(LocalizationModelContext context, DevelopmentSetup developmentSetup, IOptions<SqlLocalizationOptions> localizationOptions) : base(context, developmentSetup, localizationOptions)
        {
        }
    }

    public class MixedStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IEnumerable<IMiscibleStringLocalizerFactory> _localizerFactories;

        public MixedStringLocalizerFactory(IEnumerable<IMiscibleStringLocalizerFactory> localizerFactories)
        {
            _localizerFactories = localizerFactories;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new MixedStringLocalizer(_localizerFactories.Select(x => x.Create(baseName, location)));
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new MixedStringLocalizer(_localizerFactories.Select(x => x.Create(resourceSource)));
        }
    }


    public class MixedStringLocalizer : IStringLocalizer
    {
        private readonly IEnumerable<IStringLocalizer> _stringLocalizers;

        public MixedStringLocalizer(IEnumerable<IStringLocalizer> stringLocalizers)
        {
            _stringLocalizers = stringLocalizers;
        }

        public virtual LocalizedString this[string name]
        {
            get
            {
                var localizer = _stringLocalizers.SingleOrDefault(x => x is ResourceManagerStringLocalizer);
                var result = localizer?[name];
                if (!(result?.ResourceNotFound ?? true)) return result;

                localizer = _stringLocalizers.SingleOrDefault(x => x is SqlStringLocalizer) ?? throw new InvalidOperationException($"没有找到可用的 {nameof(IStringLocalizer)}");
                result = localizer[name];
                return result;
            }
        }

        public virtual LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var localizer = _stringLocalizers.SingleOrDefault(x => x is ResourceManagerStringLocalizer);
                var result = localizer?[name, arguments];
                if (!(result?.ResourceNotFound ?? true)) return result;

                localizer = _stringLocalizers.SingleOrDefault(x => x is SqlStringLocalizer) ?? throw new InvalidOperationException($"没有找到可用的 {nameof(IStringLocalizer)}");
                result = localizer[name, arguments];
                return result;
            }
        }

        public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var localizer = _stringLocalizers.SingleOrDefault(x => x is ResourceManagerStringLocalizer);
            var result = localizer?.GetAllStrings(includeParentCultures);
            if (!(result?.Any(x => x.ResourceNotFound) ?? true)) return result;

            localizer = _stringLocalizers.SingleOrDefault(x => x is SqlStringLocalizer) ?? throw new InvalidOperationException($"没有找到可用的 {nameof(IStringLocalizer)}");
            result = localizer?.GetAllStrings(includeParentCultures);
            return result;
        }

        [Obsolete]
        public virtual IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MixedStringLocalizer<T> : MixedStringLocalizer, IStringLocalizer<T>
    {
        public MixedStringLocalizer(IEnumerable<IStringLocalizer> stringLocalizers) : base(stringLocalizers)
        {
        }

        public override LocalizedString this[string name] => base[name];

        public override LocalizedString this[string name, params object[] arguments] => base[name, arguments];

        public override IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return base.GetAllStrings(includeParentCultures);
        }

        [Obsolete]
        public override IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
