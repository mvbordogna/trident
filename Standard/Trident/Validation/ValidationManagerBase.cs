﻿using Trident.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DA = System.ComponentModel.DataAnnotations;

namespace Trident.Validation
{
    /// <summary>
    /// Class ValidationManagerBase.
    /// Implements the <see cref="Trident.Validation.IValidationManager" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Trident.Validation.IValidationManager" />
    public abstract class ValidationManagerBase<T> : IValidationManager
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
        public async Task<List<ValidationResult>> CheckValid(ValidationContext context)
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
        /// <exception cref="Trident.Validation.ValidationRollupException"></exception>
        public async Task Validate(ValidationContext context)
        {
            var errors = new List<ValidationResult>();
            await ValidateRules(context, errors);

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
        public async Task<IEnumerable<ValidationResult>> CheckValid<TRule>(ValidationContext context) where TRule : IValidationRule
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
        /// <exception cref="Trident.Validation.ValidationRollupException">TRule;Rule not found.</exception>
        public async Task Validate<TRule>(ValidationContext context) where TRule : IValidationRule
        {
            var errors = new List<ValidationResult>();
            var rule = Rules.FirstOrDefault(x => x.GetType() == typeof(TRule));

            if (rule == null)
                throw new ArgumentOutOfRangeException("TRule", "Rule not found.");

            await rule.Run(context, errors);

            if (errors.Any())
                throw new ValidationRollupException(errors);
        }


        /// <summary>
        /// Validates the rules.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>Task.</returns>
        protected virtual async Task ValidateRules(ValidationContext context, List<ValidationResult> errors)
        {
            if (this.Rules != null)
            {
                foreach(var x in this.Rules)
                {
                   await x.Run(context, errors);
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