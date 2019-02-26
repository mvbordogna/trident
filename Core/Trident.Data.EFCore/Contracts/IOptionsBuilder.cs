using Microsoft.EntityFrameworkCore;

namespace Trident.EFCore.Contracts
{
    /// <summary>
    /// Interface IOptionsBuilder
    /// </summary>
    public interface IOptionsBuilder
    {
        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>DbContextOptions.</returns>
        DbContextOptions GetOptions(string dataSource);
    }
}
