using System;
using S=Serilog;
using Trident.Logging;

namespace Trident.Logging.Serilog
{
    /// <summary>
    /// Class SeriLogger.
    /// Implements the <see cref="TridentOptionsBuilder.Logging.ILog" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Logging.ILog" />
    public class SeriLogger : ILog
    {
        /// <summary>
        /// The log
        /// </summary>
        private readonly S.ILogger _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriLogger"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public SeriLogger(S.ILogger log) 
        {
            _log = log;
        }
        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Debug" /> level and associated exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Debug(ex, "Swallowing a mundane exception.");
        /// </example>
        public void Debug(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            var logger = (context != null) ? _log.ForContext(context) : _log;
            logger.Debug(exception, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Debug" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the class that is calling the logger.</typeparam>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Debug(ex, "Swallowing a mundane exception.");
        /// </example>
        public void Debug<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            _log.ForContext<TSourceContext>().Debug(exception, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Error" /> level and associated exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
        /// </example>
        public void Error(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            var logger = (context != null) ? _log.ForContext(context) : _log;
            logger.Error(exception, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Error" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the class that is calling the logger.</typeparam>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
        /// </example>
        public void Error<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            _log.ForContext<TSourceContext>().Error(exception, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Fatal" /> level and associated exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Fatal(ex, "Process terminating.");
        /// </example>
        public void Fatal(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            var logger = (context != null) ? _log.ForContext(context) : _log;
            logger.Fatal(exception, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Fatal" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the t source context.</typeparam>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Fatal(ex, "Process terminating.");
        /// </example>
        public void Fatal<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            _log.ForContext<TSourceContext>().Fatal(exception, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Information" /> level and associated exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
        /// </example>
        public void Information(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            var logger = (context != null) ? _log.ForContext(context) : _log;
            logger.Information(exception, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Information" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the t source context.</typeparam>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
        /// </example>
        public void Information<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            _log.ForContext<TSourceContext>().Information(exception, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Warning" /> level and associated exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
        /// </example>
        public void Warning(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
           var logger = (context != null) ? _log.ForContext(context) : _log;
            _log.Warning(exception, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Warning" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the class that is calling the logger.</typeparam>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
        /// </example>
        public void Warning<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            _log.ForContext<TSourceContext>().Warning(exception, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Debug" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the class that is calling the logger.</typeparam>
        /// <param name="logLevel">The logging level to report the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Debug(ex, "Swallowing a mundane exception.");
        /// </example>
        public void Write<TSourceContext>(LogLevel logLevel, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            S.Events.LogEventLevel seriLevel = (S.Events.LogEventLevel)Enum.Parse(typeof(S.Events.LogEventLevel), logLevel.ToString());
            _log.ForContext<TSourceContext>().Write(seriLevel, exception, messageTemplate, propertyValues);         
        }

        /// <summary>
        /// Write a log event with the <see cref="!:LogEventLevel.Debug" /> level and associated exception.
        /// </summary>
        /// <param name="logLevel">The logging level to report the event.</param>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Write(ex, "Swallowing a mundane exception.");
        /// </example>
        public void Write(LogLevel logLevel, Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues)
        {
            S.Events.LogEventLevel seriLevel = (S.Events.LogEventLevel)Enum.Parse(typeof(S.Events.LogEventLevel), logLevel.ToString());
            var logger = (context != null) ? _log.ForContext(context) : _log;
            logger.Write(seriLevel, exception, messageTemplate, propertyValues);
        }

        void ILog.SetCallContext(object functionContext)
        {
            throw new NotImplementedException();
        }
    }
}
