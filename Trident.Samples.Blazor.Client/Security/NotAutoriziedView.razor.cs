using Microsoft.AspNetCore.Components;

namespace Trident.Samples.Blazor.Client.Security
{
    public partial class NotAuthorizedView : ComponentBase
    {
        [Parameter]
        public RenderFragment Body { get; set; } = default!;

    }
}
