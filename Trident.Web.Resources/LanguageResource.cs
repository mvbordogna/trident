using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Web.Resources
{

    public class LanguageResource : ResourceBase<int>
    {
        public string Country { get; set; }
       
        public string Code { get; set; }
  
        public string Name { get; set; }
    }
}
