using Trident.Data.Contracts;
using Trident.Search;
using Trident.TestTargetProject.Domain;

namespace Trident.Core.TestRepositories
{
    public class OrganisationRepository : EFCore.EFCoreSearchRepositoryBase<Organisation>
    {
        public OrganisationRepository(ISearchResultsBuilder resultsBuilder,
            ISearchQueryBuilder queryBuilder,
            IAbstractContextFactory abstractContextFactory,
            IQueryableHelper queryableHelper
            )
            : base(resultsBuilder, queryBuilder, abstractContextFactory, queryableHelper)
        {
        }
    }
}
