using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trident.UI.Client.Contracts.Models;

namespace Trident.Samples.Contracts.Models
{
    public class OrganizationModel: GuidModelBase
    {
        
        public int Age { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }

        public OrganisationTypes OrgType { get; set; }

        public List<DepartmentModel> Departments { get; set; }
        
    }
}
