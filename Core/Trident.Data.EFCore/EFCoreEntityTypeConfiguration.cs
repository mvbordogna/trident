using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Trident.EFCore
{
    /// <summary>
    /// Class EFCoreEntityTypeConfiguration.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    public class EFCoreEntityTypeConfiguration<TEntity> where TEntity : class
    {
        /// <summary>
        /// Maps the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public virtual void Map(EntityTypeBuilder<TEntity> builder)
        {

        }
    }

    /// <summary>
    /// Class ModelBuilderExtensions.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Adds the configuration.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="configuration">The configuration.</param>
        public static void AddConfiguration<TEntity>(ModelBuilder modelBuilder, EFCoreEntityTypeConfiguration<TEntity> configuration)
            where TEntity : class
        {
            configuration.Map(modelBuilder.Entity<TEntity>());
        }
    }
}
