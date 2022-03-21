namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface ITenantResolver
    /// </summary>
    public interface ITenantResolver
    {
        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <returns>System.String.</returns>
        string GetCurrent();
    }
}