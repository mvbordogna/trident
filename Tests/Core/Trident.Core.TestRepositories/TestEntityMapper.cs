using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trident.EFCore;
using Trident.TestTargetProject.Domain;
using Trident.Extensions;
namespace Trident.Core.TestRepositories
{
    public class TestEntityMapper : EntityMapper<TestTargetProject.Domain.Organisation>
    {
        public override void Configure(EntityTypeBuilder<Organisation> modelBinding)
        {
            modelBinding.GuardIsNotNull(nameof(modelBinding));
        }
    }
}
