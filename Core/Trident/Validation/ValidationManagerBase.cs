using Trident.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident.Business;

namespace Trident.Validation
{
    /// <summary>
    /// Class ValidationManagerBase.
    /// Implements the <see cref="TridentOptionsBuilder.Validation.IValidationManager" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TridentOptionsBuilder.Validation.IValidationManager" />
    public abstract class ValidationManagerBase<T> : IValidationManager<T>
        where T : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationManagerBase{T}" /> class.
        /// </summary>
        /// <param name="rules">The rules.</param>
        protected ValidationManagerBase(IEnumerable<IValidationRule> rules)
        {
            if (rules != null)
                this.Rules = rules.OrderBy(x => x.RunOrder).ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationManagerBase{T}" /> class.
        /// </summary>
        protected ValidationManagerBase()
        {
            Rules = new List<IValidationRule>();
        }

        /// <summary>
        /// Validates the specified object to validate.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>List&lt;ValidationResult&gt;.</returns>
        public async Task<List<ValidationResult>> CheckValid(BusinessContext context)
        {
            var errors = new List<ValidationResult>();
            await ValidateRules(context, errors);
            return errors;
        }

        /// <summary>
        /// Validates the specified object to validate.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>List&lt;ValidationResult&gt;.</returns>
        /// <exception cref="ValidationRollupException"></exception>
        /// <exception cref="TridentOptionsBuilder.Validation.ValidationRollupException"></exception>
        public async Task Validate(BusinessContext context)
        {
            var errors = new List<ValidationResult>();
            await ValidateRules(context, errors);

            if (errors.Any())
                throw new ValidationRollupException(errors);
        }

        public void ValidateSync(BusinessContext context)
        {
            var errors = new List<ValidationResult>();
            ValidateRulesSync(context, errors);

            if (errors.Any())
                throw new ValidationRollupException(errors);
        }

        /// <summary>
        /// Checks the valid.
        /// </summary>
        /// <typeparam name="TRule">The type of the t rule.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>IEnumerable&lt;ValidationResult&gt;.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">TRule;Rule not found.</exception>
        public async Task<IEnumerable<ValidationResult>> CheckValid<TRule>(BusinessContext context) where TRule : IValidationRule
        {
            var errors = new List<ValidationResult>();
            var rule = Rules.FirstOrDefault(x => x.GetType() == typeof(TRule));
            if (rule == null)
                throw new ArgumentOutOfRangeException("TRule", "Rule not found.");

            await rule.Run(context, errors);
            return errors;
        }
               
        /// <summary>
        /// Validates the specified context.
        /// </summary>
        /// <typeparam name="TRule">The type of the t rule.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">TRule;Rule not found.</exception>
        /// <exception cref="ValidationRollupException"></exception>
        /// <exception cref="TridentOptionsBuilder.Validation.ValidationRollupException">TRule;Rule not found.</exception>
        public async Task Validate<TRule>(BusinessContext context) where TRule : IValidationRule
        {
            var errors = new List<ValidationResult>();
            var rule = Rules.FirstOrDefault(x => x.GetType() == typeof(TRule));

            if (rule == null)
                throw new ArgumentOutOfRangeException("TRule", "Rule not found.");

            await rule.Run(context, errors);

            if (errors.Any())
                throw new ValidationRollupException(errors);
        }

        public void ValidateSync<TRule>(BusinessContext context) where TRule : IValidationRule
        {
            var errors = new List<ValidationResult>();
            var rule = Rules.FirstOrDefault(x => x.GetType() == typeof(TRule));

            if (rule == null)
                throw new ArgumentOutOfRangeException("TRule", "Rule not found.");

            //TODO: Add Sync method to IValidationRule Interface
            //rule.RunSync(context, errors);

            if (errors.Any())
                throw new ValidationRollupException(errors);
        }

        /// <summary>
        /// Validates the rules.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>Task.</returns>
        protected virtual async Task ValidateRules(BusinessContext context, List<ValidationResult> errors)
        {
            if (this.Rules != null)
            {
                foreach (var x in this.Rules)
                {
                    await x.Run(context, errors);
                }
            }
        }

        protected virtual void ValidateRulesSync(BusinessContext context, List<ValidationResult> errors)
        {
            if (this.Rules != null)
            {
                foreach (var x in this.Rules)
                {
                    //TODO: uncomment when Sync method is implemented
                    //await x.RunSync(context, errors);
                }
            }
        }



        /// <summary>
        /// Gets the rules.
        /// </summary>
        /// <value>The rules.</value>
        protected List<IValidationRule> Rules { get; private set; }
    }
}
