using System;
using System.Collections.Generic;

namespace Trident.EFCore
{
    public interface IEntityMapFactory: IFactory
    {
        IEnumerable<IEntityMapper> GetMapsFor(Type mapType);
    }
 
}