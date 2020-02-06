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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CoreDX.Common.Util.TypeExtensions;

namespace IdentityServer.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IApplicationBuilder ConfigLocalizationFluentValidation(this IApplicationBuilder app)
        {
            ValidatorOptions.LanguageManager = new LocalizationLanguageManager(
                app.ApplicationServices.GetService<IStringLocalizerFactory>(),
                new LanguageManager(),
                app.ApplicationServices.GetService<ILogger<LocalizationLanguageManager>>());
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
                            try
                            {
                                return localizer[displayAttribute.Name];
                            }
                            catch (Exception ex)
                            {
                                app.ApplicationServices.GetService<ILogger<LocalizationLanguageManager>>()
                                    .LogError(ex, "There is an error on get localization display name, get default display name instead.");

                                //return defaultDisplayNameResolver?.Invoke(type, info, lambda);
                            }
                        }
                    }
                }

                return defaultDisplayNameResolver?.Invoke(type, info, lambda);
            }
        }

        /// <summary>
        /// 指定基于ValidatorOptions.LanguageManager的验证失败消息
        /// </summary>
        /// <typeparam name="T">验证对象的类型</typeparam>
        /// <typeparam name="TProperty">验证属性的类型</typeparam>
        /// <param name="rule">验证规则</param>
        /// <param name="message">消息内容</param>
        /// <param name="includeTypeName">消息中是否包括被验证实例的类型名</param>
        /// <returns>验证规则</returns>
        public static IRuleBuilderOptions<T, TProperty> WithLanguageManagedMessage<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, string message, bool includeTypeName = false)
        {
            return rule.WithMessage(((instanceOfValidated, valueOfValidatedProperty) =>
                ValidatorOptions.LanguageManager.GetString($"{(includeTypeName ? instanceOfValidated.GetType().FullName : string.Empty)}:{message}")));
        }
    }

    /// <summary>
    /// 本地化语言管理器
    /// 使用IStringLocalizerFactory
    /// </summary>
    public class LocalizationLanguageManager : ILanguageManager
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly ILanguageManager _languageManager;
        private readonly ILogger _logger;

        public LocalizationLanguageManager(IStringLocalizerFactory stringLocalizerFactory, ILanguageManager languageManager, ILogger logger)
        {
            _stringLocalizerFactory = stringLocalizerFactory;
            _languageManager = languageManager;
            _logger = logger;
        }

        public string GetString(string key, CultureInfo culture = null)
        {
            var lKey = _languageManager.GetString(key, culture);
            lKey = lKey.IsNullOrWhiteSpace() ? key : lKey;
            var stringLocalizer = _stringLocalizerFactory.Create(lKey, "+FluentValidation");

            try
            {
                return stringLocalizer[lKey];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is an error on get localization string, get default string with provided languageManager instead.");
            }

            return _languageManager.GetString(key, culture);
        }

        public bool Enabled { get; set; } = true;
        public CultureInfo Culture { get; set; }
    }
}
