using Trident.Contracts;

namespace Trident.Rest
{
    /// <summary>
    /// An IExternalReference implementation that has the same id on both sides.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IExternalReference{T, T}" />
    public class SameExternalReference<T> : IExternalReference<T, T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SameExternalReference{T}"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public SameExternalReference(T id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>The external identifier.</value>
        public T ExternalId
        {
            get => Id;
            set => Id = value;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public T Id { get; set; }
    }
}