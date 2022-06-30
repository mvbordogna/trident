using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Trident.Contracts.Api.Validation
{
    /// <summary>
    /// Class ValidationResult.
    /// </summary>
    [Serializable]
    public class ValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        public ValidationResult() { }

        /// <summary>
        /// The member names
        /// </summary>
        private readonly List<string> _memberNames = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="code">The code.</param>
        public ValidationResult(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="propertyNames">The property names.</param>
        public ValidationResult(string message, params string[] propertyNames) : this(message)
        {
            if (propertyNames.Any())
                _memberNames = propertyNames.Select(ConvertToCamelCase).ToList();
            else
                _memberNames = new List<string> { "Unspecified" };

        }

        public ValidationResult(params string[] propertyNames)
        {
            if (propertyNames.Any())
                _memberNames = propertyNames.Select(ConvertToCamelCase).ToList();
            else
                _memberNames = new List<string> { "Unspecified" };

        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets the member names.
        /// </summary>
        /// <value>The member names.</value>
        public IEnumerable<string> MemberNames => _memberNames;


        /// <summary>
        /// Adds the name of the member.
        /// </summary>
        /// <param name="name">The name.</param>
        public void AddMemberName(string name)
        {
            _memberNames.Add(ConvertToCamelCase(name));
        }


        /// <summary>
        /// Converts to camelcase.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        protected static string ConvertToCamelCase(string target)
        {
            if ((target?.Length ?? 0) <= 0) return target;
            if (target.All(char.IsUpper)) return target.ToLower();
            return char.ToLowerInvariant(target[0]) + target.Substring(1);
        }
    }


    public class ValidationResult<TErrorCodes> : ValidationResult
       where TErrorCodes : struct
    {
        public ValidationResult() : base() { }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public TErrorCodes? ErrorCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        public ValidationResult(TErrorCodes? code = null, string message = null) : base()
        {
            ErrorCode = code.GetValueOrDefault();
            Message = !ErrorCode.Equals(0) && Message == null
                ? GetErrorCodeDescriptionOrDefault(ErrorCode.GetValueOrDefault())
                : message ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="propertyNames">The property names.</param>
        public ValidationResult(TErrorCodes code, params string[] propertyNames) : base(propertyNames)
        {
            ErrorCode = code;
            Message = GetErrorCodeDescriptionOrDefault(ErrorCode.GetValueOrDefault());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="propertyNames">The property names.</param>
        public ValidationResult(string message, TErrorCodes? code = null, params string[] propertyNames) : base(message, propertyNames)
        {
            ErrorCode = code;
            Message = message;
        }

        /// <summary>
        /// Gets the error code description or default.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>System.String.</returns>
        protected static string GetErrorCodeDescriptionOrDefault(TErrorCodes code)
        {
            var codeString = code.ToString();
            return code.GetType().GetMember(codeString).First()
                 .GetCustomAttribute<DescriptionAttribute>()?.Description ?? codeString;
        }
    }



    /// <summary>
    ///   <br />
    /// </summary>
    /// <typeparam name="TErrorCodes">The type of the error codes.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class ValidationResult<TErrorCodes, TEntity> : ValidationResult<TErrorCodes>
         where TErrorCodes : struct
    {
        public ValidationResult() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult{T}"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="propertyNameExps">The property name exps.</param>
        public ValidationResult(string message, TErrorCodes code, params Expression<Func<TEntity, object>>[] propertyNameExps)
          : base()
        {
            ErrorCode = ErrorCode;
            Message = message;
            MemberExpressions = propertyNameExps;
        }

        public ValidationResult(TErrorCodes code)
            : base(code) { }

        public ValidationResult(TErrorCodes code, params Expression<Func<TEntity, object>>[] propertyNameExps)
        : base()
        {
            ErrorCode = code;
            Message = GetErrorCodeDescriptionOrDefault(ErrorCode.GetValueOrDefault());
            MemberExpressions = propertyNameExps;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="propertyNames">The property names.</param>
        public ValidationResult(TErrorCodes code, params string[] propertyNames) : base(code, propertyNames) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="propertyNames">The property names.</param>
        public ValidationResult(string message, TErrorCodes? code = null, params string[] propertyNames)
            : base(message, code, propertyNames) { }

        /// <summary>
        /// Gets or sets the member expressions.
        /// </summary>
        /// <value>The member expressions.</value>
        public IEnumerable<Expression<Func<TEntity, object>>> MemberExpressions { get; set; } = new List<Expression<Func<TEntity, object>>>();

        /// <summary>
        /// Gets the object graph path.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="Field">The field.</param>
        /// <returns>System.String.</returns>
        private static string GetObjectGraphPath<TSource>(Expression<Func<TSource, object>> Field)
        {
            string propertyPath = string.Empty;

            var expression = Field.Body as MemberExpression ?? ((UnaryExpression)Field.Body).Operand as MemberExpression;

            while (expression.NodeType == ExpressionType.MemberAccess)
            {
                propertyPath = $"{ ConvertToCamelCase(expression.Member.Name)}.{propertyPath}";

                if (expression.Expression.NodeType != ExpressionType.MemberAccess)
                    break;

                expression = expression.Expression as MemberExpression ?? ((UnaryExpression)expression.Expression).Operand as MemberExpression;
            }

            return propertyPath.Remove(propertyPath.Length - 1);
        }

        /// <summary>
        /// Applies the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void ApplyExpression(Expression<Func<TEntity, object>> expression)
        {
            AddMemberName(GetObjectGraphPath(expression));
        }
    }
}
