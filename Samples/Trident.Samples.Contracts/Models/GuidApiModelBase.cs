using System;
using Trident.Contracts.Api;
using Trident.Contracts.Api.Client;

namespace Trident.Samples.Contracts.Models
{
    public class GuidApiModelBase : ApiModelBase<Guid>, IGuidModelBase
    {
        public string ToJson()
        {
            return String.Empty;
        }
    }
}
