using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Trident.Data;
using Trident.Domain;

namespace Trident.TestTargetProject.Domain
{
    [UseSharedDataSource(DBSources.CosmosDB)]
    //[Container("Organisations", "0")]
    public class Organisation : EntityGuidBase
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }

        [NotMapped]
        public OrganisationTypes OrgType { get; set; }

        public OrgStatus Status { get; set; }

        public List<Department> Departments { get; set; }
    }


    [UseSharedDataSource(DBSources.CosmosDB)]
    //[Container("Organisations", "0")]
    public class OrgStatus : EntityGuidBase
    {

        public string OrgType { get; set; }

    }

    public enum OrganisationTypes
    {
        Corp = 1,
        SCorp = 2,
        LLC = 3
    }
}
