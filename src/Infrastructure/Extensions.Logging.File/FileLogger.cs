using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Extensions.Logging.File
{
    public class FileLogger : ILogger
    {
        protected static string Delimiter = new string(new[] { (char)1 });
        public FileLogger(string categoryName)
        {
            Name = categoryName;
        }
        class Disposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        readonly Disposable _disposableInstance = new Disposable();
        public IDisposable BeginScope<TState>(TState state)
        {
            return _disposableInstance;
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return MinLevel <= logLevel;
        }
        public void Reload()
        {
            _expires = true;
        }

        public string Name { get; }

        public LogLevel MinLevel { get; set; }
        public string FileDirectoryPath { get; set; }
        public string FileNameTemplate { get; set; }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;
            var msg = formatter(state, exception);
            Write(logLevel, eventId, msg, exception);
        }
        void Write(LogLevel logLevel, EventId eventId, string message, Exception ex)
        {
            EnsureInitFile();

            //TODO 提高效率 队列写！！！
            var log = String.Concat(DateTime.Now.ToString("HH:mm:ss"), '[', logLevel.ToString(), ']', '[',
                  Thread.CurrentThread.ManagedThreadId.ToString(), ',', eventId.Id.ToString(), ',', eventId.Name, ']',
                  Delimiter, message, Delimiter, ex?.ToString());
            lock (this)
            {
                Sw.WriteLine(log);
            }
        }

        bool _expires = true;
        string _fileName;
        protected StreamWriter Sw;
        void EnsureInitFile()
        {
            if (CheckNeedCreateNewFile())
            {
                lock (this)
                {
                    if (CheckNeedCreateNewFile())
                    {
                        InitFile();
                        _expires = false;
                    }
                }
            }
        }
        bool CheckNeedCreateNewFile()
        {
            if (_expires)
            {
                return true;
            }
            //TODO 使用 RollingType判断是否需要创建文件。提高效率！！！
            if (_fileName != DateTime.Now.ToString(FileNameTemplate))
            {
                return true;
            }
            return false;
        }
        void InitFile()
        {
            if (!Directory.Exists(FileDirectoryPath))
            {
                Directory.CreateDirectory(FileDirectoryPath);
            }
            string path;
            int i = 0;
            do
            {
                _fileName = DateTime.Now.ToString(FileNameTemplate);
                path = Path.Combine(FileDirectoryPath, _fileName + "_" + i + ".log");
                i++;
            } while (System.IO.File.Exists(path));
            var oldSw = Sw;
            Sw = new StreamWriter(new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read),
                Encoding.UTF8) {AutoFlush = true};
            if (oldSw != null)
            {
                try
                {
                    Sw.Flush();
                    Sw.Dispose();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}