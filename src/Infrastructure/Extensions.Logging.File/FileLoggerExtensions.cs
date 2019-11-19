using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace Extensions.Logging.File
{
    public static class FileLoggerExtensions
    {
        ///// <summary>Adds a console logger named 'Console' to the factory.</summary>
        ///// <param name="builder">The <see cref="T:Microsoft.Extensions.Logging.ILoggingBuilder" /> to use.</param>
        //public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
        //{
        //    builder.AddConfiguration();
        //    builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
        //    builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<FileLoggerOptions>, FileLoggerOptionsSetup>());
        //    builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<FileLoggerOptions>, LoggerProviderOptionsChangeTokenSource<FileLoggerOptions, FileLoggerProvider>>());
        //    return builder;
        //}

        ///// <summary>Adds a console logger named 'Console' to the factory.</summary>
        ///// <param name="builder">The <see cref="T:Microsoft.Extensions.Logging.ILoggingBuilder" /> to use.</param>
        ///// <param name="configure"></param>
        //public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
        //{
        //    if (configure == null)
        //        throw new ArgumentNullException(nameof(configure));
        //    builder.AddFile();
        //    builder.Services.Configure<FileLoggerOptions>(configure);
        //    return builder;
        //}

        //add 日志文件创建规则，分割规则，格式化规则，过滤规则 to appsettings.json
        public static ILoggerFactory AddFile(this ILoggerFactory factory, IConfiguration configuration)
        {
            return AddFile(factory, new FileLoggerSettings(configuration));
        }
        public static ILoggerFactory AddFile(this ILoggerFactory factory, FileLoggerSettings fileLoggerSettings)
        {
            factory.AddProvider(new FileLoggerProvider(fileLoggerSettings));
            return factory;
        }
    }
}