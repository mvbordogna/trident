using Newtonsoft.Json;
using System;
using Trident.Contracts.Api.Client;

namespace Trident.UI.Client.Contracts.Models
{
    public class GuidModelBase : ModelBase<Guid>, IGuidModelBase
    {
        public T Evaluate<T>(bool result, T trueValue, T falseValue)
        {
            return result ? trueValue : falseValue;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
