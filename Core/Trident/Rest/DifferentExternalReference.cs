using Trident.Contracts;

namespace Trident.Rest
{
    /// <summary>
    /// An IExternalReference implementation that has different ids on either sides.
    /// </summary>
    /// <typeparam name="TLocal">The type of the t local.</typeparam>
    /// <typeparam name="TTarget">The type of the t target.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IExternalReference{TLocal, TTarget}" />
    public class DifferentExternalReference<TLocal, TTarget> : IExternalReference<TLocal, TTarget>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DifferentExternalReference{TLocal, TTarget}"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="target">The target.</param>
        public DifferentExternalReference(TLocal id, TTarget target)
        {
            Id = id;
            ExternalId = target;
        }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>The external identifier.</value>
        public TTarget ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public TLocal Id { get; set; }
    }
}