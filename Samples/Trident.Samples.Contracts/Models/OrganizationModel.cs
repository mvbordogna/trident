using System;
using System.Collections.Generic;

namespace Trident.Samples.Contracts.Models
{

    public class OrganizationModel : GuidApiModelBase
    {

        public int Age { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }

        public OrganisationTypes OrgType { get; set; }

        public List<DepartmentModel> Departments { get; set; }

    }
}
