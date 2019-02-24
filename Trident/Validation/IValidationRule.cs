﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trident.Validation
{

    /// <summary>
    /// Interface IValidationRule
    /// </summary>
    public interface IValidationRule
    {
        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <value>The ordinal.</value>
        int RunOrder { get; }

        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>Task.</returns>
        Task Run(object context, List<ValidationResult> errors);
    }

    /// <summary>
    /// Interface IValidationRule
    /// Implements the <see cref="Trident.Validation.IValidationRule" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Trident.Validation.IValidationRule" />
    public interface IValidationRule<in T> : IValidationRule
        where T : Validation.ValidationContext
    {
        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>ValidationError.</returns>
        Task Run(T context, List<ValidationResult> errors);
    }
}
