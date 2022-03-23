using System;

namespace Trident.UI.Blazor.Models;

public class ApplicationContext
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public UserContext User { get; set; }
    public LookupContext Lookups { get; set; }
}
