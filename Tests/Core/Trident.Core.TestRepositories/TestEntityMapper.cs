using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trident.EFCore;
using Trident.TestTargetProject.Domain;
using Trident.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Trident.Core.TestRepositories
{
    public class OrganisationEntityMapper : EntityMapper<TestTargetProject.Domain.Organisation>
    {
        public override void Configure(EntityTypeBuilder<Organisation> modelBinding)
        {
            modelBinding.GuardIsNotNull(nameof(modelBinding));
            // modelBinding.HasMany(x => x.Departments).WithOne().HasForeignKey(x=> x.OrganisationId);
            modelBinding.OwnsMany(x => x.Departments).WithOwner().HasForeignKey(x => x.OrganisationId);
        }
    }

    public class DepartmentEntityMapper : EntityMapper<TestTargetProject.Domain.Department>
    {
        public override void Configure(EntityTypeBuilder<Department> modelBinding)
        {
            modelBinding.GuardIsNotNull(nameof(modelBinding));
            modelBinding.ToContainer("Organisations");
        }
    }
}
