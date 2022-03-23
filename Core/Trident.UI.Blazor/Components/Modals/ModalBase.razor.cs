using Microsoft.AspNetCore.Components;

namespace Trident.UI.Blazor.Components.Modals
{
    public partial class ModalBase : ComponentBase
    {
        private bool _show;
        private string _modalDisplay = "none";
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string ModalStyle { get; set; } = "";
        [Parameter]
        public bool Show
        {
            get => _show;
            set
            {
                _show = value;
                _modalDisplay = value ? "block;" : "none";
            }
        }

        public void Open()
        {
            Show = true;
            ModalStyle = "Show";
            StateHasChanged();
        }

        public void Close()
        {
            Show = false;
            ModalStyle = "";
            StateHasChanged();
        }
    }
}
