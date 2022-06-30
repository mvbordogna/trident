using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Trident.Contracts.Api;
using Trident.Samples.Contracts;
using Trident.Samples.Domain.Entities;

namespace Trident.Samples.AzureFunctions.Models.Organization
{
    public class Organization : ApiModelBase<Guid>
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }

        [NotMapped]
        public OrganisationTypes OrgType { get; set; }

        public List<DepartmentEntity> Departments { get; set; }
    }
}
