using Trident.Contracts.Enums;
using Trident.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trident.Business;

namespace Trident.Validation
{
    /// <summary>
    /// Class PropertyExpressionValidationRule.
    /// Implements the <see cref="TridentOptionsBuilder.Validation.ValidationRuleBase{TContext}" />
    /// </summary>
    /// <typeparam name="TContext">The type of the t context.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Validation.ValidationRuleBase{TContext}" />
    public abstract class PropertyExpressionValidationRule<TContext, TEntity, TErrorCodes> : ValidationRuleBase<TContext>
         where TContext : BusinessContext<TEntity>
         where TEntity : Entity
         where TErrorCodes:struct
    {
        /// <summary>
        /// The property rules
        /// </summary>
        private Dictionary<string, PropertyRule<TEntity>> PropertyRules = new Dictionary<string, PropertyRule<TEntity>>();

        /// <summary>
        /// The default message format
        /// </summary>
        private static string DefaultMessageFormat = "Property {0} of {1} did not meet the expressed condition(s) of {2}";


        /// <summary>
        /// Configures the rules.
        /// </summary>
        protected abstract void ConfigureRules(TContext context);

        /// <summary>
        /// Adds the rule.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="rule">The rule.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorCode">The error code.</param>
        protected void AddRule(string propertyName, Expression<Func<TEntity, bool>> rule, string errorMessage = null, TErrorCodes? errorCode = null)
        {
            PropertyRules[propertyName] = new PropertyRule<TEntity>()
            {
                PropertyName = propertyName,
                Rule = rule.Compile(),
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                RuleExpression = rule
            };
        }


        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>Task.</returns>
        public override Task Run(TContext context, List<ValidationResult> errors)
        {
            ConfigureRules(context);
            foreach (var rule in PropertyRules.Keys)
            {
                var property = PropertyRules[rule];

                if (!property.Rule(context.Target))
                {
                    var expressionString = property.RuleExpression.ToString();
                    var msg = property.ErrorMessage ?? string.Format(DefaultMessageFormat, property.PropertyName, typeof(TEntity).Name, property.RuleExpression);
                    var code = property.ErrorCode;
                    errors.Add(new ValidationResult<TErrorCodes, TEntity>(msg, code));
                }
            }

            return Task.CompletedTask;
        }
        /// <summary>
        /// Class PropertyRule.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class PropertyRule<T>
        {
            /// <summary>
            /// Gets or sets the name of the property.
            /// </summary>
            /// <value>The name of the property.</value>
            public string PropertyName { get; set; }
            /// <summary>
            /// Gets or sets the rule.
            /// </summary>
            /// <value>The rule.</value>
            public Func<T, bool> Rule { get; set; }

            /// <summary>
            /// Gets or sets the rule expression.
            /// </summary>
            /// <value>The rule expression.</value>
            public Expression<Func<T, bool>> RuleExpression { get; set; }

            /// <summary>
            /// Gets or sets the error message.
            /// </summary>
            /// <value>The error message.</value>
            public string ErrorMessage { get; set; }
            /// <summary>
            /// Gets or sets the error code.
            /// </summary>
            /// <value>The error code.</value>
            public TErrorCodes? ErrorCode { get; set; }
        }
    }
}
