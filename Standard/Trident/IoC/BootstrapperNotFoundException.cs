using System;
using System.Runtime.Serialization;

namespace Trident.IoC
{
    /// <summary>
    /// Class BootstrapperNotFoundException.
    /// Implements the <a href="http://msdn.microsoft.com/en-us/library/system.applicationexception.aspx" target="_blank">ApplicationException</a>
    /// Implements the <see cref="System.ApplicationException" />
    /// </summary>
    /// <seealso cref="System.ApplicationException" />
    public class BootstrapperNotFoundException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperNotFoundException" /> class.
        /// </summary>
        public BootstrapperNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperNotFoundException" /> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public BootstrapperNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
        public BootstrapperNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperNotFoundException" /> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        public BootstrapperNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}