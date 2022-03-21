using System.Collections.Generic;
using System.Threading.Tasks;
using Trident.Business;
using Trident.Domain;

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
        Task<List<ValidationResult>> CheckValid(BusinessContext context);

        /// <summary>
        /// Validates the specified context.
        /// Throws ValidationRollupException if error are found.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ValidationRollupException"></exception>
        Task Validate(BusinessContext context);

        void ValidateSync(BusinessContext context);

        /// <summary>
        /// Checks the valid.
        /// </summary>
        /// <typeparam name="TRule">The type of the t rule.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>IEnumerable&lt;ValidationResult&gt;.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">TRule;Rule not found.</exception>
        Task<IEnumerable<ValidationResult>> CheckValid<TRule>(BusinessContext context) where TRule : IValidationRule;

        /// <summary>
        /// Validates the specified context.
        /// </summary>
        /// <typeparam name="TRule">The type of the t rule.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">TRule;Rule not found.</exception>
        /// <exception cref="TridentOptionsBuilder.Validation.ValidationRollupException"></exception>
        Task Validate<TRule>(BusinessContext context) where TRule : IValidationRule;

        void ValidateSync<TRule>(BusinessContext context) where TRule : IValidationRule;

    }

    public interface IValidationManager<T> : IValidationManager
        where T : Entity
    {
    }
}
