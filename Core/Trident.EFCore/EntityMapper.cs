using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Trident.EFCore
{
    public abstract class EntityMapper<T> : IEntityMapper<T>
      where T : class
    {
        public abstract void Configure(EntityTypeBuilder<T> modelBinding);

        public void Configure(EntityTypeBuilder modelBinding)
        {
            this.Configure(modelBinding as EntityTypeBuilder<T>);
        }
    }
}
