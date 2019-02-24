namespace Trident.Data.EntityFramework.Contracts
{
    /// <summary>
    /// Interface IModelBuilderFactory
    /// </summary>
    public interface IModelBuilderFactory
    {
        /// <summary>
        /// Gets a ModelBuilder for the specified data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>IDbModelBuilder.</returns>
        IDbModelBuilder Get(string dataSource);
    }
}
