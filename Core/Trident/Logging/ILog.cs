using System;

namespace Trident.Logging
{
    /// <summary>
    /// Interface ILog
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Debug" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the class that is calling the logger.</typeparam>
        /// <param name="logLevel">The logging level to report the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Debug(ex, "Swallowing a mundane exception.");
        /// </example>
        void Write<TSourceContext>(LogLevel logLevel, Exception exception = null, string messageTemplate = null, params object[] propertyValues);


        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Debug" /> level and associated exception.
        /// </summary>
        /// <param name="logLevel">The logging level to report the event.</param>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Write(ex, "Swallowing a mundane exception.");
        /// </example>
        void Write(LogLevel logLevel, Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues);


        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Debug" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the class that is calling the logger.</typeparam>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Debug(ex, "Swallowing a mundane exception.");
        /// </example>
        void Debug<TSourceContext>(Exception exception=null, string messageTemplate=null, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Information" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the t source context.</typeparam>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
        /// </example>
        void Information<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Warning" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the class that is calling the logger.</typeparam>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
        /// </example>
        void Warning<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Error" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the class that is calling the logger.</typeparam>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
        /// </example>
        void Error<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Fatal" /> level and associated exception.
        /// </summary>
        /// <typeparam name="TSourceContext">The type of the t source context.</typeparam>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Fatal(ex, "Process terminating.");
        /// </example>
        void Fatal<TSourceContext>(Exception exception = null, string messageTemplate = null, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Debug" /> level and associated exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Debug(ex, "Swallowing a mundane exception.");
        /// </example>
        void Debug(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Information" /> level and associated exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
        /// </example>
        void Information(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Warning" /> level and associated exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
        /// </example>
        void Warning(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Error" /> level and associated exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
        /// </example>
        void Error(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Fatal" /> level and associated exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Fatal(ex, "Process terminating.");
        /// </example>
        void Fatal(Type context = null, Exception exception = null, string messageTemplate = null, params object[] propertyValues);

        /// <summary>
        /// Uses the function context to set the internal adapter.
        /// </summary>
        /// <param name="functionContext"></param>
        void SetCallContext(object functionContext);

    }
}