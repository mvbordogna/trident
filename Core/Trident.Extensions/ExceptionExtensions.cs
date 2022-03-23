using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;

namespace Trident.Extensions
{
    /// <summary>
    ///     Extension method class for <see cref="Exception"/>.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        ///     Returns a flattened list of exceptions from a single root exception <see cref="Exception"/>.
        /// </summary>
        /// <param name="exception">Parent exception.</param>
        /// <returns>Flattened list of exceptions from a single root exception.</returns>  
        public static IEnumerable<Exception> ExpandException(this Exception exception)
        {
            exception.GuardIsNotNull(nameof(exception));

            do
            {
                yield return exception;

                if (exception is AggregateException aggregateException)
                {
                    foreach (var inerException in aggregateException.InnerExceptions)
                    {
                        var nextEx = ExpandException(inerException);
                        foreach (var ex in nextEx)
                            yield return ex;
                    }
                }

                exception = exception.InnerException;
            } while (exception != null);
        }
    }
}
