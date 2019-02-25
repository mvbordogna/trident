using Trident.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Trident.Validation
{
    /// <summary>
    /// Class ValidationResult.
    /// </summary>
    [Serializable]
    public class ValidationResult
    {
        /// <summary>
        /// The member names
        /// </summary>
        private readonly List<string> _memberNames = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        protected ValidationResult(ErrorCodes code)
        {
            ErrorCode = code;
            var description = GetErrorCodeDescriptionOrDefault(code);
            Message = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="propertyNames">The property names.</param>
        public ValidationResult(ErrorCodes code, params string[] propertyNames) : this(code)
        {
            if (propertyNames.Any())
                _memberNames = propertyNames.Select(ConvertToCamelCase).ToList();
            else
                _memberNames = new List<string> { "Unspecified" };

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="code">The code.</param>
        public ValidationResult(string message, ErrorCodes code)
        {
            ErrorCode = code;
            Message = message;
        }

        /// <summary>
        /// Gets the error code description or default.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>System.String.</returns>
        private static string GetErrorCodeDescriptionOrDefault(ErrorCodes code)
        {
            var codeString = code.ToString();
            return code.GetType().GetMember(codeString).First()
                 .GetCustomAttribute<DescriptionAttribute>()?.Description ?? codeString;
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

        /// <summary>
        /// Gets the member names.
        /// </summary>
        /// <value>The member names.</value>
        public IEnumerable<string> MemberNames => _memberNames;

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; protected set; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public ErrorCodes ErrorCode { get; private set; }

        /// <summary>
        /// Adds the name of the member.
        /// </summary>
        /// <param name="name">The name.</param>
        public void AddMemberName(string name)
        {
            this._memberNames.Add(ConvertToCamelCase(name));
        }
    }

    /// <summary>
    /// Class ValidationResult.
    /// Implements the <see cref="Trident.Validation.ValidationResult" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Trident.Validation.ValidationResult" />
    public class ValidationResult<T> : ValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult{T}"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="propertyNameExps">The property name exps.</param>
        public ValidationResult(ErrorCodes code, params Expression<Func<T, object>>[] propertyNameExps)
          : base(code)
        {
            this.MemberExpressions = propertyNameExps;
        }

        /// <summary>
        /// Gets or sets the member expressions.
        /// </summary>
        /// <value>The member expressions.</value>
        public IEnumerable<Expression<Func<T, object>>> MemberExpressions { get; set; }

        /// <summary>
        /// Gets the object graph path.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="Field">The field.</param>
        /// <returns>System.String.</returns>
        private static string GetObjectGraphPath<TSource>(Expression<Func<TSource, object>> Field)
        {
            string propertyPath = string.Empty;

            var expression = (Field.Body as MemberExpression ?? ((UnaryExpression)Field.Body).Operand as MemberExpression);

            while (expression.NodeType == ExpressionType.MemberAccess)
            {
                propertyPath = $"{ ConvertToCamelCase(expression.Member.Name)}.{propertyPath}";

                if (expression.Expression.NodeType != ExpressionType.MemberAccess)
                    break;

                expression = (expression.Expression as MemberExpression ?? ((UnaryExpression)expression.Expression).Operand as MemberExpression);
            }

            return propertyPath.Remove(propertyPath.Length - 1);
        }

        /// <summary>
        /// Applies the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void ApplyExpression(Expression<Func<T, object>> expression)
        {
            AddMemberName(GetObjectGraphPath(expression));
        }
    }
}

