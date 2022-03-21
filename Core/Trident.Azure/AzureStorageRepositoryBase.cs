namespace Trident.Azure
{
    /// <summary>
    /// Class AzureStorageRepositoryBase.
    /// </summary>
    /// <typeparam name="TContext">The type of the t context.</typeparam>
    public abstract class AzureStorageRepositoryBase<TContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageRepositoryBase{TContext}" /> class.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        protected AzureStorageRepositoryBase(TContext clientContext)
        {
            ClientContext = clientContext;
        }
        /// <summary>
        /// Gets the client context.
        /// </summary>
        /// <value>The client context.</value>
        protected TContext ClientContext { get; private set; }
    }
}
