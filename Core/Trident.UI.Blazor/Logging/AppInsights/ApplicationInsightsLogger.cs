using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Trident.UI.Blazor.Logging.AppInsights
{
    public interface IApplicationInsightsLogger : ILogger { }
    public interface IApplicationInsightsLogger<T> : IApplicationInsightsLogger, ILogger<T>
        where T : class
    { }

    public class ApplicationInsightsLogger : IApplicationInsightsLogger
    {

        private readonly string _categoryName;
        private readonly IApplicationInsights _applicationInsights;
        private readonly ILoggingConfiguration _logConfig;

        /// <summary>Initializes a new instance of the <see cref="ApplicationInsightsLogger"/> class</summary>
        /// <param name="applicationInsights">Instance to use for transmitting logging messages</param>
        [ActivatorUtilitiesConstructor]
        public ApplicationInsightsLogger(ILoggingConfiguration logConfig, IApplicationInsights applicationInsights)
            : this(null, logConfig, applicationInsights)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ApplicationInsightsLogger"/> class</summary>
        /// <param name="categoryName">Category of the logger, stored in the CategoryName in customDimensions if set</param>
        /// <param name="applicationInsights">Instance to use for transmitting logging messages</param>
        public ApplicationInsightsLogger(string categoryName, ILoggingConfiguration logConfig, IApplicationInsights applicationInsights)
        {
            _categoryName = categoryName;
            _applicationInsights = applicationInsights;
            _logConfig = logConfig ?? new LoggingConfiguration() { LogLevel = LogLevel.Warning };
        }

        /// <summary>Include the logger category name in customDimensions under the 'CategoryName' key</summary>
        public bool IncludeCategoryName { get; set; }

        /// <summary>Include scope information in CustomDimensions</summary>
        public bool IncludeScopes { get; set; }

        /// <summary>Set the active scope provider</summary>
        internal IExternalScopeProvider ScopeProvider { private get; set; } = new LoggerExternalScopeProvider();

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var severity = GetSeverityLevel(logLevel);
            var message = formatter(state, exception);
            var customDimensions = GetCustomDimensions(state, eventId);

            if (exception is null)
            {
                _applicationInsights.TrackTrace(message, severity, customDimensions);
                return;
            }

            var error = new Error
            {
                Name = exception.GetType().Name,
                Message = exception.Message,
                Stack = exception.StackTrace
            };
            _applicationInsights.TrackException(error, $"{eventId}", severity, customDimensions);
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None
                && (int)_logConfig.LogLevel <= (int)logLevel;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state) => ScopeProvider.Push(state);

        private Dictionary<string, object> GetCustomDimensions<TState>(TState state, EventId eventId)
        {
            var result = new Dictionary<string, object>();



            ApplyScopes(result);
            ApplyLogState(result, state, eventId);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyLogState<TState>(Dictionary<string, object> customDimensions, TState state, EventId eventId)
        {
            if (IncludeCategoryName && !string.IsNullOrEmpty(_categoryName))
                customDimensions["CategoryName"] = _categoryName;

            if (eventId.Id != 0)
                customDimensions["EventId"] = eventId.Id.ToString(CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(eventId.Name))
                customDimensions["EventName"] = eventId.Name;

            if (state is IReadOnlyCollection<KeyValuePair<string, object>> stateDictionary)
                ApplyDictionary(customDimensions, stateDictionary);
        }

        private void ApplyScopes(Dictionary<string, object> customDimensions)
        {
            if (!IncludeScopes)
                return;

            var scopeBuilder = new StringBuilder();
            ScopeProvider.ForEachScope(ApplyScope, (customDimensions, scopeBuilder));

            if (scopeBuilder.Length > 0)
                customDimensions["Scope"] = scopeBuilder.ToString();

            static void ApplyScope(object scope, (Dictionary<string, object> data, StringBuilder scopeBuilder) result)
            {
                if (scope is IReadOnlyCollection<KeyValuePair<string, object>> scopeDictionary)
                {
                    ApplyDictionary(result.data, scopeDictionary);
                    return;
                }

                result.scopeBuilder.Append(" => ").Append(scope);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyDictionary(Dictionary<string, object> target, IReadOnlyCollection<KeyValuePair<string, object>> source)
        {
            foreach (var kvp in source)
            {
                var key = kvp.Key == "{OriginalFormat}" ? "OriginalFormat" : kvp.Key;
                target[key] = Convert.ToString(kvp.Value, CultureInfo.InvariantCulture);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SeverityLevel GetSeverityLevel(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Trace => SeverityLevel.Verbose,
            LogLevel.Debug => SeverityLevel.Verbose,
            LogLevel.Information => SeverityLevel.Information,
            LogLevel.Warning => SeverityLevel.Warning,
            LogLevel.Error => SeverityLevel.Error,
            LogLevel.Critical => SeverityLevel.Critical,
            _ => SeverityLevel.Verbose
        };

    }
    public class ApplicationInsightsLogger<T> : ApplicationInsightsLogger, IApplicationInsightsLogger<T>
        where T : class
    {
        public ApplicationInsightsLogger(ILoggingConfiguration logConfig, IApplicationInsights applicationInsights) :
            base(nameof(T), logConfig, applicationInsights)
        {
        }
    }

}

