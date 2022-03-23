using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Contracts.Routes
{
    
    public class OrganizationRoutes
    {
        
        public const string GetAllMethod = "get";
        public const string GetAll = "organization/all";

        public const string CreateMethod = "post";
        public const string Create = "organization";

        public const string GetByIdMethod = "get";
        public const string GetById = "organization/{id}/getbyid";
    }
}
