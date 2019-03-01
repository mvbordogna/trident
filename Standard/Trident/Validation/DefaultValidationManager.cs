using System.Collections.Generic;
using Trident.Domain;

namespace Trident.Validation
{
    public class DefaultValidationManager<T> : ValidationManagerBase<T>
        where T: Entity
    {
        public DefaultValidationManager(IEnumerable<IValidationRule<ValidationContext<T>>> rules) : base(rules)
        {
        }
    }
}
