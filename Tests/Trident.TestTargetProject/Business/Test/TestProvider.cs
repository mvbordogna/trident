using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Text;
using Trident.Business;
using Trident.Search;
using Trident.TestTargetProject.Domain;

namespace Trident.TestTargetProject
{
    public class TestProvider : ProviderBase<Guid, Organisation>
    {
        public TestProvider(ISearchRepository<Organisation> repository) 
            : base(repository)
        {
        }
    }
}
