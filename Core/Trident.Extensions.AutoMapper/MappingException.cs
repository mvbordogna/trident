using System;

namespace Trident.Mapper
{
    /// <summary>
    /// Class MappingExceptionException.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class MappingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingException" /> class.
        /// </summary>
        public MappingException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MappingException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MappingException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingException" /> class.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="DestinationType">Type of the destination.</param>
        /// <param name="exception">The exception.</param>
        public MappingException(Type sourceType, Type DestinationType, Exception exception)
            : base(string.Format("Mapping exception occurred while mapping from {0} to {1}", sourceType.FullName, DestinationType.FullName), exception) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected MappingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

    }
}
