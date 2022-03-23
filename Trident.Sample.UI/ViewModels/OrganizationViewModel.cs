using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trident.Samples.Contracts.Models;

namespace Trident.Sample.UI.ViewModels
{
    public class OrganizationViewModel
    {
        
            public OrganizationViewModel()
            {
                Departments = new List<DepartmentModel>();
            }
            public string InformationText { get; set; }
            public List<DepartmentModel> Departments { get; set; }
        
    }
}
