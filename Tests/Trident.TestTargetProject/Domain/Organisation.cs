using System;
using Trident.Data;
using Trident.Domain;

namespace Trident.TestTargetProject.Domain
{
    [UseSharedDataSource(DBSources.CosmosDB)]
    [Container("Organisations")]
    public class Organisation : EntityGuidBase
    {
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }

        public OrganisationTypes OrgType { get; set; }

        public OrgStatus Status { get; set; }
    }


    [UseSharedDataSource(DBSources.CosmosDB)]
    [Container("Organisations")]
    public class OrgStatus : EntityGuidBase
    {

        public OrganisationTypes OrgType { get; set; }

    }

    public enum OrganisationTypes
    {
        Corp = 1,
        SCorp = 2,
        LLC = 3
    }
}
