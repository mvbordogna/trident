using System;

namespace Trident.Contracts.Api.Client
{
    public interface IGuidModelBase
    {
        Guid Id { get; set; }
        string ToJson();
    }
}
