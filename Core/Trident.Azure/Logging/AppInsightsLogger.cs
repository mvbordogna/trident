using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Trident.Logging;

namespace Trident.Azure.Logging
{
    [ExcludeFromCodeCoverage]
    public class AppInsightsLogger : ILog
    {
        private Guid InstanceId { get; } = Guid.NewGuid();
        private static string RequestId = "Request-Id";
        private ILogger _logger;

        public AppInsightsLogger() { }


        private class ImportantHeaders
        {
            [JsonProperty("Request-Id")]
            public string RequestId { get; set; }

            [JsonProperty("Accept-Encoding")]
            public string AcceptedEncoding { get; set; }

        }

        void ILog.SetCallContext(object functionContext)
        {
            if (functionContext is FunctionContext ctx)
            {
                _logger = ctx.GetLogger(ctx.FunctionDefinition.Name);
                Guid invocationId;
                ImportantHeaders headers = null;
                if (ctx.BindingContext.BindingData.ContainsKey("Headers"))
                {
                    var headerJson = ctx.BindingContext.BindingData["Headers"];
                    headers = JsonConvert.DeserializeObject<ImportantHeaders>((string)headerJson);
                }
              

                if (headers != null && !string.IsNullOrWhiteSpace(headers.RequestId))
                {
                    if (Guid.TryParse(headers.RequestId, out invocationId))
                    {
                        Trace.CorrelationManager.ActivityId = invocationId;
                        this.CorrelationId = headers.RequestId;
                    }
                }
                else
                {
                    if (Guid.TryParse(ctx.InvocationId, out invocationId))
                    {
                        Trace.CorrelationManager.ActivityId = invocationId;
                        this.CorrelationId = ctx.InvocationId;
                    }
                }

                EventType = ctx.FunctionDefinition.EntryPoint.Replace($".{ctx.FunctionDefinition.Name}", string.Empty);
                EventSource =ctx.FunctionDefinition.Name; 
            }
        }



        public string CorrelationId { get; private set; }
        public string EventType { get; private set; }
        public string EventSource { get; private set; }
              

        public void LogMetric(string name, double value, Dictionary<string, object> properties = null)
        {
            _logger.LogMetric(name, value, properties);
        }

        private Microsoft.Extensions.Logging.LogLevel GetLogLevel(Trident.Logging.LogLevel severity)
        {
            var level = Microsoft.Extensions.Logging.LogLevel.Trace;
            Enum.TryParse(severity.ToString(), out level);

            return level;
        }

        public void Write<TSourceContext>(Trident.Logging.LogLevel logLevel, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            _logger.Log(GetLogLevel(logLevel), exception, messageTemplate, propertyValues);
        }

        public void Write(Trident.Logging.LogLevel logLevel, Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            _logger.Log(GetLogLevel(logLevel), exception, messageTemplate, propertyValues);
        }

        public void Debug<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            Write<TSourceContext>(Trident.Logging.LogLevel.Debug, exception, messageTemplate, propertyValues);
        }

        public void Information<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            Write<TSourceContext>(Trident.Logging.LogLevel.Information, exception, messageTemplate, propertyValues);
        }

        public void Warning<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            Write<TSourceContext>(Trident.Logging.LogLevel.Warning, exception, messageTemplate, propertyValues);
        }

        public void Error<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            Write<TSourceContext>(Trident.Logging.LogLevel.Error, exception, messageTemplate, propertyValues);
        }

        public void Fatal<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            Write<TSourceContext>(Trident.Logging.LogLevel.Fatal, exception, messageTemplate, propertyValues);
        }

        public void Debug(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            Write(Trident.Logging.LogLevel.Debug, context, exception, messageTemplate, propertyValues);
        }

        public void Information(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            Write(Trident.Logging.LogLevel.Information, context, exception, messageTemplate, propertyValues);
        }

        public void Warning(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            Write(Trident.Logging.LogLevel.Warning, context, exception, messageTemplate, propertyValues);
        }

        public void Error(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            Write(Trident.Logging.LogLevel.Error, context, exception, messageTemplate, propertyValues);
        }

        public void Fatal(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            Write(Trident.Logging.LogLevel.Fatal, context, exception, messageTemplate, propertyValues);
        }
    }
}
