﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trident.Validation
{
    /// <summary>
    /// Class IValidationManager.
    /// </summary>
    public interface IValidationManager
    {
        /// <summary>
        /// Validates the specified object to validate.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>List&lt;ValidationResult&gt;.</returns>
        Task<List<ValidationResult>> CheckValid(ValidationContext context);

        /// <summary>
        /// Validates the specified context.
        /// Throws ValidationRollupException if error are found.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ValidationRollupException"></exception>
        Task Validate(ValidationContext context);

        /// <summary>
        /// Checks the valid.
        /// </summary>
        /// <typeparam name="TRule">The type of the t rule.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>IEnumerable&lt;ValidationResult&gt;.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">TRule;Rule not found.</exception>
        Task<IEnumerable<ValidationResult>> CheckValid<TRule>(ValidationContext context) where TRule : IValidationRule;

        /// <summary>
        /// Validates the specified context.
        /// </summary>
        /// <typeparam name="TRule">The type of the t rule.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">TRule;Rule not found.</exception>
        /// <exception cref="Trident.Validation.ValidationRollupException"></exception>
        Task Validate<TRule>(ValidationContext context) where TRule : IValidationRule;

    }
}