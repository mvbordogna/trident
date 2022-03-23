using System;
using System.Collections.Generic;
using System.Text;
using Trident.Data;
using Trident.Domain;

namespace Trident.Samples.Domain.Entities
{
    //[UseSharedDataSource(DBSources.CosmosDB)]
    //[Container("Organisations")]
    public class DepartmentEntity : EntityGuidBase
    {
        public string Name { get; set; }
        public Guid OrganisationId { get; set; }
    }
}
