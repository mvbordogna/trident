using Microsoft.EntityFrameworkCore;

namespace Trident.EFCore.Contracts
{
    /// <summary>
    /// Interface IEFCoreModelBuilderFactory
    /// </summary>
    public interface IEFCoreModelBuilderFactory
    {
        /// <summary>
        /// Gets the Model To use when creating the DbContext
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>IEFCoreModelBuilder.</returns>
        IEFCoreModelBuilder GetBuilder(string dataSource);           
    }
}
