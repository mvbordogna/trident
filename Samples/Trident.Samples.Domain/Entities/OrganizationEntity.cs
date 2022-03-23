using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Trident.Contracts.Constants;
using Trident.Data;
using Trident.Domain;
using Trident.Samples.Contracts;

namespace Trident.Samples.Domain.Entities
{
    [UseSharedDataSource(DBSources.CosmosDB)]
    [Container(Containers.Infrastructure, nameof(DocumentType), Containers.Partitions.Organizations)]
    [Discriminator(Containers.Discriminators.Organization, Property = nameof(EntityType))]
    public class OrganizationEntity : DocumentDbEntityBase
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }

        [NotMapped]
        public OrganisationTypes OrgType { get; set; }

        public List<DepartmentEntity> Departments { get; set; }
    }


}
