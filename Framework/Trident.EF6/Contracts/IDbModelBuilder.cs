using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Trident.EF6.Contracts
{
    /// <summary>
    /// Interface IDbModelBuilder
    /// </summary>
    public interface IDbModelBuilder
    {
        /// <summary>
        /// Builds the model mappings.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        void BuildModelMappings(DbModelBuilder modelBuilder);
        /// <summary>
        /// Gets the compiled.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>DbCompiledModel.</returns>
        DbCompiledModel GetCompiled(IDbConnection connection);
    }
}