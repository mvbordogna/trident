using Microsoft.EntityFrameworkCore;

namespace Trident.EFCore.Contracts
{
    /// <summary>
    /// Interface IDbModelBuilder
    /// </summary>
    public interface IEFCoreModelBuilder
    {
        /// <summary>
        /// Builds the model mappings.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        void AppendModelMappings(ModelBuilder modelBuilder, IEntityMapFactory mapFactory);

    }
}