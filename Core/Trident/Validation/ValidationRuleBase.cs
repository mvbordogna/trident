using System.Collections.Generic;
using System.Threading.Tasks;
using Trident.Business;

namespace Trident.Validation
{
    /// <summary>
    /// Class ValidationRuleBase.
    /// Implements the <see cref="TridentOptionsBuilder.Validation.IValidationRule{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TridentOptionsBuilder.Validation.IValidationRule{T}" />
    public abstract class ValidationRuleBase<T> : IValidationRule<T>
        where T : BusinessContext
    {
        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <value>The ordinal.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public abstract int RunOrder { get; }


        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>Task.</returns>
        public async Task Run(object context, List<ValidationResult> errors)
        {
            await this.Run((T)context, errors);
        }

        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>ValidationError.</returns>
        public abstract Task Run(T context, List<ValidationResult> errors);

    }
}
