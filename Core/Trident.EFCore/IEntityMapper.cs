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
}
