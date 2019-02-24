using System;
using System.Runtime.Serialization;

namespace Trident.Workflow
{
    /// <summary>
    /// Class WorkFlowCancelledException.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class WorkFlowCancelledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkFlowCancelledException"/> class.
        /// </summary>
        public WorkFlowCancelledException()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkFlowCancelledException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellingTaskType">Type of the cancelling task.</param>
        public WorkFlowCancelledException(string message, Type cancellingTaskType) : base(message)
        {
            CancellingTaskType = cancellingTaskType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkFlowCancelledException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public WorkFlowCancelledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkFlowCancelledException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected WorkFlowCancelledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Gets the type of the cancelling task.
        /// </summary>
        /// <value>The type of the cancelling task.</value>
        public Type CancellingTaskType { get; }
    }
}