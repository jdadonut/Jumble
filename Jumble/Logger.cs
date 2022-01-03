using System;
using Microsoft.Extensions.Logging;

namespace Jumble
{
    public class Logger : ILogger
    {
        LogLevel _logLevel;
        string fmt = "[{0}][{1}]\\> {2}";
        public Logger(string fmt, LogLevel logLevel = LogLevel.Information)
        {
            if (fmt.Contains("{0}") && fmt.Contains("{1}") && fmt.Contains("{2}"))
                this.fmt = fmt;
            else
            {
                Console.WriteLine("Invalid format string");
            }
            _logLevel = logLevel;
        }
        public Logger(LogLevel logLevel = LogLevel.Information)
        {
            _logLevel = logLevel;
        }
        private string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        public void Log(params string[] message)
        {
            if (message.Length == 0 || LogLevel.Information < _logLevel)
                return;
            Console.WriteLine(string.Format(fmt, "LOG", GetTime(), string.Join(", ", message)));
        }
        public void Warn(params string[] message)
        {
            if (message.Length == 0 || LogLevel.Warning < _logLevel)
                return;
            Console.WriteLine(string.Format(fmt, "WARN", GetTime(), string.Join(", ", message)));
        }
        public void Error(params string[] message)
        {
            if (message.Length == 0 || LogLevel.Error < _logLevel)
                return;
            Console.WriteLine(string.Format(fmt, "ERROR", GetTime(), string.Join(", ", message)));
        }
        public void Fatal(params string[] message)
        {
            if (message.Length == 0)
                return;
            Console.WriteLine(string.Format(fmt, "FATAL", GetTime(), string.Join(", ", message)));
        }
        public void Info(params string[] message)
        {
            if (message.Length == 0 || LogLevel.Debug < _logLevel)
                return;
            Console.WriteLine(string.Format(fmt, "INFO", GetTime(), string.Join(", ", message)));
        }
        public void Debug(params string[] message)
        {
            if (message.Length == 0 || LogLevel.Trace < _logLevel)
                return;
            Console.WriteLine(string.Format(fmt, "DEBUG", GetTime(), string.Join(", ", message)));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel < _logLevel)
                return;
            Console.WriteLine(string.Format(fmt, logLevel.ToString(), GetTime(), formatter(state, exception)));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _logLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
    public class LFactory : ILoggerFactory
    {
        public void Dispose()
        {
            
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger();
        }

        public void AddProvider(ILoggerProvider provider)
        {
            
        }
    }
}