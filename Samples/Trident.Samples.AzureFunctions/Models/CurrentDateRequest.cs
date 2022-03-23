using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Samples.AzureFunctions.Models
{
    public class CurrentDateRequest
    {
        public DateTime CurrentDate { get; set; }
        public string RequestType { get; set; }
    }
}
