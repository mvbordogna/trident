using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Samples.AzureFunctions.Models.Organization
{
    public class Department
    {
        public string Name { get; set; }
        public Guid OrganisationId { get; set; }
    }
}
