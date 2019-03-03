using System.Collections.Generic;
using Trident.Business;
using Trident.Domain;

namespace Trident.Validation
{
    public class DefaultValidationManager<T> : ValidationManagerBase<T>, IValidationManager<T>
        where T : Entity
    {
        public DefaultValidationManager(IEnumerable<IValidationRule<BusinessContext<T>>> rules) : base(rules)
        {
        }
    }
}
