using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using Trident.IoC;
using Trident.UI.Blazor.Logging.AppInsights;
using Trident.UI.Blazor.Logging.Browser;
using Trident.UI.Client.Logging;

namespace Trident.UI.Blazor.Logging
{
    public class BlazorClientLoggerProvider : ILoggerProvider
    {
        private static readonly Func<string, LogLevel, bool> TrueFilter = (cat, level) => true;

        private readonly Func<string, LogLevel, bool> filter;
        private readonly IIoCServiceLocator _locator;
        private ConcurrentDictionary<string, BlazorClientLogger> loggers;

        
        public BlazorClientLoggerProvider(IIoCServiceLocator locator)
        {
            _locator = locator;
        }


        //public BlazorClientLoggerProvider(Func<string, LogLevel, bool> filter)
        //{
        //    this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        //}

        public ILogger CreateLogger(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            if (this.loggers == null)
            {
                this.loggers = new ConcurrentDictionary<string, BlazorClientLogger>();
            }

            return this.loggers.GetOrAdd(categoryName, this.CreateLoggerImplementation);
        }

        public void Dispose() => this.loggers?.Clear();

        private BlazorClientLogger CreateLoggerImplementation(string name)
        {
            var appInsights = _locator.Get<IApplicationInsights>();
            var logConfig = _locator.Get<ILoggingConfiguration>();
            var appInsightsLogger = new ApplicationInsightsLogger(name, logConfig, appInsights);
            var browserWriter = _locator.Get<IBrowserConsoleWriter>();
            var browserConsolLogger = new BrowserConsoleLogger(name, logConfig, browserWriter);
            return new BlazorClientLogger(name, logConfig, browserConsolLogger, appInsightsLogger);
        }

        private Func<string, LogLevel, bool> GetFilter(string name)
        {
            if (this.filter != null)
            {
                return this.filter;
            }

            return TrueFilter;
        }
    }
}
