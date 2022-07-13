using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Contracts.Api
{
    public abstract class ApiModelBase<TId>
    {
        public TId Id { get; set; }
    }
}
