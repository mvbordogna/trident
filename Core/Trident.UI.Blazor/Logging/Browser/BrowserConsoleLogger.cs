using Microsoft.Extensions.Logging;
using System;
using System.Text;
using Trident.UI.Client.Logging;

namespace Trident.UI.Blazor.Logging.Browser
{
    public interface IBrowserConsoleLogger : ILogger { }
    public interface IBrowserConsoleLogger<T> : ILogger<T>, IBrowserConsoleLogger { }

    public class BrowserConsoleLogger<T> : BrowserConsoleLogger, IBrowserConsoleLogger<T>
        where T : class
    {
        public BrowserConsoleLogger(ILoggingConfiguration logConfig, IBrowserConsoleWriter jsRuntime) : base(logConfig, jsRuntime)
        {
        }
    }

    public class BrowserConsoleLogger : IBrowserConsoleLogger
    {
        private const string _loglevelPadding = ": ";
        private static readonly string _messagePadding = new(' ', GetLogLevelString(LogLevel.Information).Length + _loglevelPadding.Length);
        private static readonly string _newLineWithMessagePadding = Environment.NewLine + _messagePadding;
        private static readonly StringBuilder _logBuilder = new StringBuilder();

        private readonly string _name;
        private readonly ILoggingConfiguration _logConfig;
        private readonly IBrowserConsoleWriter _jsRuntime;

        public BrowserConsoleLogger(ILoggingConfiguration logConfig, IBrowserConsoleWriter jsRuntime)
            : this(string.Empty, logConfig, jsRuntime) // Cast for DI
        {
        }

        public BrowserConsoleLogger(string name, ILoggingConfiguration logConfig, IBrowserConsoleWriter jsRuntime)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _logConfig = logConfig ?? new LoggingConfiguration() { LogLevel = LogLevel.Warning };
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoOpDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None
                && (int)_logConfig.LogLevel <= (int)logLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                WriteMessage(logLevel, _name, eventId.Id, message, exception);
            }
        }

        private void WriteMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            lock (_logBuilder)
            {
                try
                {
                    CreateDefaultLogMessage(_logBuilder, logLevel, logName, eventId, message, exception);
                    var formattedMessage = _logBuilder.ToString();
                    _jsRuntime.Write(logLevel, formattedMessage);
                }
                finally
                {
                    _logBuilder.Clear();
                }
            }
        }

        private void CreateDefaultLogMessage(StringBuilder logBuilder, LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            logBuilder.Append(GetLogLevelString(logLevel));
            logBuilder.Append(_loglevelPadding);
            logBuilder.Append(logName);
            logBuilder.Append('[');
            logBuilder.Append(eventId);
            logBuilder.Append(']');

            if (!string.IsNullOrEmpty(message))
            {
                // message
                logBuilder.AppendLine();
                logBuilder.Append(_messagePadding);

                var len = logBuilder.Length;
                logBuilder.Append(message);
                logBuilder.Replace(Environment.NewLine, _newLineWithMessagePadding, len, message.Length);
            }

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                // exception message
                logBuilder.AppendLine();
                logBuilder.Append(exception.ToString());
            }
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private class NoOpDisposable : IDisposable
        {
            public static NoOpDisposable Instance = new NoOpDisposable();

            public void Dispose() { }
        }
    }


}
