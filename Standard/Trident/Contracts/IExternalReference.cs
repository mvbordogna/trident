namespace Trident.Contracts
{
    /// <summary>
    /// Interface that defines the minimum memmbers for maintaining a map between an external (third-party) and local (internal) entity.
    /// </summary>
    /// <typeparam name="TLocal">The type of the t local.</typeparam>
    /// <typeparam name="TExternal">The type of the t external.</typeparam>
    public interface IExternalReference<TLocal, TExternal>
    {
        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>The external identifier.</value>
        TExternal ExternalId { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        TLocal Id { get; set; }
    }
}