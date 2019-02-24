using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Trident.Sample.Web.Models
{
    // Models returned by MeController actions.
    public class GetViewModel
    {
        public string Hometown { get; set; }
    }
}