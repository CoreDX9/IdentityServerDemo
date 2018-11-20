using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Extensions.Logging.File
{
    public class FileLoggerProvider : ILoggerProvider
    {
        readonly FileLoggerSettings _configuration;
        readonly ConcurrentDictionary<string, InitLoggerModel> _loggerKeys = new ConcurrentDictionary<string, InitLoggerModel>();
        readonly ConcurrentDictionary<string, FileLogger> _loggers = new ConcurrentDictionary<string, FileLogger>();

        public FileLoggerProvider(FileLoggerSettings configuration)
        {
            _configuration = configuration;
            _configuration.ChangeToken.RegisterChangeCallback(p =>
            {
                //appsettings.json changed. reload settings.
                _configuration.Reload();

                //update loggers settings form new settings
                foreach (var item in _loggers.Values)
                {
                    InitLoggerModel model = new InitLoggerModel();
                    InitLoggerSettings(item.Name, model);
                    InitLogger(model, item);
                }

            }, null);
        }
        public ILogger CreateLogger(string categoryName)
        {
            var loggerKey = _loggerKeys.GetOrAdd(categoryName, p =>
            {
                InitLoggerModel model = new InitLoggerModel();
                InitLoggerSettings(categoryName, model);
                return model;
            });
            var key = loggerKey.FileDirectoryPath + loggerKey.FileNameTemplate;
            return _loggers.GetOrAdd(key, p =>
            {
                var logger = new FileLogger(categoryName);
                InitLogger(loggerKey, logger);
                return logger;
            });
        }

        private static void InitLogger(InitLoggerModel model, FileLogger logger)
        {
            logger.FileNameTemplate = model.FileNameTemplate;
            logger.FileDirectoryPath = model.FileDirectoryPath;
            logger.MinLevel = model.MinLevel;
        }

        class InitLoggerModel
        {
            public LogLevel MinLevel { get; set; }
            public string FileDirectoryPath { get; set; }
            public string FileNameTemplate { get; set; }

            public override int GetHashCode() => MinLevel.GetHashCode() + FileDirectoryPath.GetHashCode() + FileNameTemplate.GetHashCode();

            public override bool Equals(object obj)
            {
                var b = obj as InitLoggerModel;
                if (b == null)
                    return false;
                return MinLevel == b.MinLevel && FileDirectoryPath == b.FileDirectoryPath && FileNameTemplate == b.FileNameTemplate;
            }

        }
        private void InitLoggerSettings(string categoryName, InitLoggerModel model)
        {
            model.MinLevel = LogLevel.Debug;
            var keys = GetKeys(categoryName);
            foreach (var item in keys)
            {
                var switchV = _configuration.GetSwitch(item);
                if (switchV.Item1)
                {
                    model.MinLevel = switchV.Item2;
                    break;
                }
            }
            model.FileDirectoryPath = _configuration.DefaultPath;
            foreach (var item in keys)
            {
                var switchV = _configuration.GetDiretoryPath(item);
                if (switchV.Item1)
                {
                    model.FileDirectoryPath = switchV.Item2;
                    break;
                }
            }
            model.FileNameTemplate = _configuration.DefaultFileName;
            foreach (var item in keys)
            {
                var switchV = _configuration.GetFileName(item);
                if (switchV.Item1)
                {
                    model.FileNameTemplate = switchV.Item2;
                    break;
                }
            }
        }

        IEnumerable<string> GetKeys(string categoryName)
        {
            while (!String.IsNullOrEmpty(categoryName))
            {
                // a.b.c
                //--result
                // a.b.c，a.b，a，Default
                yield return categoryName;
                var last = categoryName.LastIndexOf('.');
                if (last <= 0)
                {
                    yield return "Default";
                    yield break;
                }
                Debug.WriteLine(categoryName + "--" + last);
                categoryName = categoryName.Substring(0, last);
            }
        }
        public void Dispose()
        {
        }
    }
}