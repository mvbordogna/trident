using System;
using Trident.Contracts.Enums;

namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface ITenantContextFactory
    /// </summary>
    public interface ITenantContextFactory
    {
        /// <summary>
        /// Gets the specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="companyCode">The company code.</param>
        /// <returns>IContext.</returns>
        IContext Get(Type entityType, TenantDataSource dataSource, string companyCode);
    }
}
