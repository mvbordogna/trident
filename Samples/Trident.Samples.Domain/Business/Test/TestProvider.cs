using System;
using Trident.Business;
using Trident.Samples.Domain.Entities;
using Trident.Search;

namespace Trident.TestTargetProject
{
    public class TestProvider : ProviderBase<Guid, OrganizationEntity>
    {
        public TestProvider(ISearchRepository<OrganizationEntity> repository)
            : base(repository)
        {
        }
    }
}
