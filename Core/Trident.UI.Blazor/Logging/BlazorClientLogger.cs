using Microsoft.Extensions.Logging;
using Trident.UI.Blazor.Logging;
using Trident.UI.Blazor.Logging.AppInsights;
using Trident.UI.Blazor.Logging.Browser;
using System;

namespace Trident.UI.Blazor.Logging
{
    public interface IBlazorClientLogger : ILogger { }

    public class BlazorClientLogger : IBlazorClientLogger
    {
        private string _name;
        private readonly IBrowserConsoleLogger _browserConsoleLogger;
        private readonly IApplicationInsightsLogger _applicationInsightsLogger;
        private readonly ILoggingConfiguration _logConfig;

        public BlazorClientLogger(string name, ILoggingConfiguration logConfig, IBrowserConsoleLogger browserConsoleLogger, IApplicationInsightsLogger applicationInsightsLogger)
        {
            _name = name;
            _browserConsoleLogger = browserConsoleLogger;
            _applicationInsightsLogger = applicationInsightsLogger;
            _logConfig = logConfig ?? new LoggingConfiguration() { LogLevel = LogLevel.Warning };
        }




        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None
                && (int)_logConfig.LogLevel <= (int)logLevel;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state) => null;

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
                _browserConsoleLogger.Log(logLevel, eventId.Id, message, exception, formatter);
                _applicationInsightsLogger.Log(logLevel, eventId.Id, message, exception, formatter);
            }
        }
    }
    public class BlazorClientLogger<T> : BlazorClientLogger, ILogger<T>
    {
        public BlazorClientLogger(ILoggingConfiguration logConfig, IBrowserConsoleLogger browserConsoleLogger, IApplicationInsightsLogger applicationInsightsLogger)
            : base(nameof(T), logConfig, browserConsoleLogger, applicationInsightsLogger) { }
    }
}
