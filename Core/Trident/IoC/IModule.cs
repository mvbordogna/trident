using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.IoC
{
    public interface IModule
    {
        void Configure(IIoCProvider builder);
    }    
}
