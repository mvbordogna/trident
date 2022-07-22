using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Trident.Contracts.Configuration;
using Trident.Extensions;
using Trident.UI.Blazor.Logging.AppInsights;
using Trident.UI.Blazor.Logging.Browser;
using Trident.UI.Client.Logging;

namespace Trident.UI.Blazor.Logging
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Adds a logger that target the browser's console output
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// </summary>
        public static ILoggingBuilder AddBlazorClientLogging(this ILoggingBuilder builder, ILoggingConfiguration logConfig, IAppInsightsConfig appInsightsConfig = null)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, BlazorClientLoggerProvider>());

            // HACK: Override the hardcoded ILogger<> injected by Blazor
            builder.Services.Add(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(BlazorClientLogger<>)));
            builder.Services.Add(ServiceDescriptor.Singleton(typeof(IBrowserConsoleLogger), typeof(BrowserConsoleLogger)));
            builder.Services.Add(ServiceDescriptor.Singleton(typeof(IApplicationInsightsLogger), typeof(ApplicationInsightsLogger)));
            builder.Services.Add(ServiceDescriptor.Singleton(typeof(IBrowserConsoleLogger<>), typeof(BrowserConsoleLogger<>)));
            builder.Services.Add(ServiceDescriptor.Singleton(typeof(IApplicationInsightsLogger<>), typeof(ApplicationInsightsLogger<>)));
            builder.Services.Add(ServiceDescriptor.Singleton(typeof(IApplicationInsights), typeof(AppInsights.ApplicationInsights)));
            builder.Services.Add(ServiceDescriptor.Singleton(typeof(IBrowserConsoleWriter), typeof(BrowserConsoleWriter)));
            builder.Services.Add(ServiceDescriptor.Singleton(appInsightsConfig));
            builder.Services.Add(ServiceDescriptor.Singleton(logConfig));
            return builder;
        }

        public static WebAssemblyHostBuilder ConfigureLogging(this WebAssemblyHostBuilder builder, IAppSettings settings, LogLevel logLevel = LogLevel.None)
        {
            var appInsightsConnStr = settings.ConnectionStrings["AppInsights"]?.ToString();
            var appInsightsKey = string.Empty;

            appInsightsConnStr.ToDictionary(';', '=', true)?.TryGetValue("instrumentationkey", out appInsightsKey);

            if (logLevel == LogLevel.None)
            {
                Enum.TryParse(settings["LogLevel"]?.ToString(), out logLevel);
            }

            builder.Logging.AddBlazorClientLogging(
                new LoggingConfiguration()
                {
                    LogLevel = logLevel
                },

                new AppInsightsConfig()
                {
                    ConnectionString = appInsightsConnStr,
                    InstrumentationKey = appInsightsKey,
                    DisableFetchTracking = false,
                    EnableCorsCorrelation = true,
                    EnableRequestHeaderTracking = true,
                    EnableResponseHeaderTracking = true
                }
            );

            return builder;
        }
    }
}
