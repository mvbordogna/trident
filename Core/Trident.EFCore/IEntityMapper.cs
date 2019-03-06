using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Trident.EFCore
{

    public interface IEntityMapper
    {
        void Configure(EntityTypeBuilder modelBinding);
    }

    public interface IEntityMapper<T> : IEntityMapper
        where T : class
    {
        void Configure(EntityTypeBuilder<T> modelBinding);
    }


    public abstract class EntityMapperBase<T> : IEntityMapper<T>
      where T : class
    {
        public abstract void Configure(EntityTypeBuilder<T> modelBinding);

        public void Configure(EntityTypeBuilder modelBinding)
        {
            this.Configure(modelBinding as EntityTypeBuilder<T>);
        }
    }
}
