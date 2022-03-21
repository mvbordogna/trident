using Microsoft.EntityFrameworkCore;

namespace Trident.EFCore.Contracts
{
    /// <summary>
    /// Interface IOptionsFactory
    /// </summary>
    public interface IOptionsFactory
    {
        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>DbContextOptions.</returns>
        DbContextOptions GetOptions(string dataSource);
    }


}
