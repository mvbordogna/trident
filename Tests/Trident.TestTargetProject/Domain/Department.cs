using System;
using System.Collections.Generic;
using System.Text;
using Trident.Data;
using Trident.Domain;

namespace Trident.TestTargetProject.Domain
{
    //[UseSharedDataSource(DBSources.CosmosDB)]
    //[Container("Organisations")]
    public class Department : EntityGuidBase
    {
       public string Name { get; set; }
        public Guid OrganisationId { get; set; }
    }
}
