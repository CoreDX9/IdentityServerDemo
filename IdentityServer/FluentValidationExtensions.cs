using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using FluentValidation.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Util.TypeExtensions;

namespace IdentityServer
{
    public static class FluentValidationExtensions
    {
        public static IApplicationBuilder ConfigLocalizationFluentValidation(this IApplicationBuilder app)
        {
            ValidatorOptions.LanguageManager = new LocalizationLanguageManager(app.ApplicationServices.GetService<IStringLocalizerFactory>(), new LanguageManager());
            var defaultDisplayNameResolver = ValidatorOptions.DisplayNameResolver;
            ValidatorOptions.DisplayNameResolver = LocalizationDisplayNameResolver;

            return app;

            string LocalizationDisplayNameResolver(Type type, MemberInfo info, LambdaExpression lambda)
            {
                var displayAttribute = info.GetCustomAttributes<DisplayAttribute>().FirstOrDefault();
                if (displayAttribute?.GetName().IsNullOrWhiteSpace() == false)
                {
                    var stringLocalizerFactory = app.ApplicationServices.GetService<IStringLocalizerFactory>();

                    if (stringLocalizerFactory != null)
                    {
                        var localizationOptions = app.ApplicationServices.GetService<IOptions<MvcDataAnnotationsLocalizationOptions>>();
                        var dataAnnotationLocalizerProvider = localizationOptions?.Value?.DataAnnotationLocalizerProvider;
                        var localizer = dataAnnotationLocalizerProvider?.Invoke(info.DeclaringType, stringLocalizerFactory);

                        if (localizer != null)
                        {
                            return localizer[displayAttribute.Name];
                        }
                    }
                }

                return defaultDisplayNameResolver?.Invoke(type, info, lambda);
            }
        }
    }

    public class LocalizationLanguageManager : ILanguageManager
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly ILanguageManager _languageManager;

        public LocalizationLanguageManager(IStringLocalizerFactory stringLocalizerFactory, ILanguageManager languageManager)
        {
            _stringLocalizerFactory = stringLocalizerFactory;
            _languageManager = languageManager;
        }

        public string GetString(string key, CultureInfo culture = null)
        {
            var lKey = _languageManager.GetString(key, culture);
            lKey = lKey.IsNullOrWhiteSpace() ? key : lKey;
            var stringLocalizer = _stringLocalizerFactory.Create(lKey, "+FluentValidation");
            return stringLocalizer[lKey];
        }

        public bool Enabled { get; set; } = true;
        public CultureInfo Culture { get; set; }
    }
}
