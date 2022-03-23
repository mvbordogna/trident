using Microsoft.AspNetCore.Components;

namespace Trident.UI.Blazor.Components.Security
{
    public partial class NotAuthorizedView : ComponentBase
    {
        [Parameter]
        public RenderFragment Body { get; set; } = default!;

    }
}
