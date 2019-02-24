using Trident.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Trident.Validation
{
    /// <summary>
    /// Class Validation Roll up Exception.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class ValidationRollupException : Exception
    {
        /// <summary>
        /// The _validation results
        /// </summary>
        private List<ValidationResult> _validationResults;
        /// <summary>
        /// The default error message
        /// </summary>
        public const string DefaultErrorMessage = "Validation error occurred.";
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRollupException" /> class.
        /// </summary>
        public ValidationRollupException() : base(DefaultErrorMessage)
        {
            _validationResults = new List<ValidationResult>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRollupException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ValidationRollupException(string message)
            : base(string.IsNullOrWhiteSpace(message) ? DefaultErrorMessage : message)
        {
            _validationResults = new List<ValidationResult>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRollupException" /> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="code">The code.</param>
        public ValidationRollupException( string memberName, ErrorCodes code) : this()
        {
            _validationResults = new List<ValidationResult>()
            {
                new ValidationResult(code, memberName)
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRollupException" /> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="code">The code.</param>
        public ValidationRollupException(string errorMessage, string memberName, ErrorCodes code) : this(errorMessage)
        {
            _validationResults = new List<ValidationResult>()
            {
                new ValidationResult(code, memberName)
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRollupException" /> class.
        /// </summary>
        /// <param name="validationError">The validation error.</param>
        public ValidationRollupException(ValidationResult validationError) : this()
        {
            if (validationError != null)
                _validationResults = new[] { validationError }.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRollupException" /> class.
        /// </summary>
        /// <param name="validationErrors">The validation errors.</param>
        public ValidationRollupException(IEnumerable<ValidationResult> validationErrors) : this()
        {
            if (validationErrors != null)
                _validationResults = validationErrors.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRollupException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="validationErrors">The validation errors.</param>
        public ValidationRollupException(string message, IEnumerable<ValidationResult> validationErrors) : this(message)
        {
            if (validationErrors != null)
                _validationResults = validationErrors.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRollupException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected ValidationRollupException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }


        /// <summary>
        /// Gets or sets the validation results.
        /// </summary>
        /// <value>The validation results.</value>
        public IEnumerable<ValidationResult> ValidationResults => _validationResults;


        /// <summary>
        /// Adds the result.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void AddResult(ValidationResult exception)
        {
            _validationResults.Add(exception);
        }


    }
}
