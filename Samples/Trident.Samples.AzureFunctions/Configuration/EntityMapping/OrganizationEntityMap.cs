using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trident.EFCore;
using Trident.Samples.Domain.Entities;

namespace Trident.Samples.AzureFunctions.Configuration.EntityMapping
{
    public class OrganizationEntityMap : EntityMapper<OrganizationEntity>, IEntityMapper<OrganizationEntity>
    {
        public override void Configure(EntityTypeBuilder<OrganizationEntity> modelBinding)
        {
            modelBinding.OwnsMany(x => x.Departments,
                doc =>
                {
                    doc.ToJsonProperty("Departments");
                });

            modelBinding.Property(x => x.OrgType).HasConversion<string>();

        }
    }
}
