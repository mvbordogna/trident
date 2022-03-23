using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using RenderFragment = Microsoft.AspNetCore.Components.RenderFragment;

namespace Trident.UI.Blazor.Components.Buttons
{
    public partial class ButtonComponent : BaseRazorComponent
    {
        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public BsButtonType ButtonType { get; set; } = BsButtonType.Primary;

        [Parameter]
        public EventCallback ParentMethod { get; set; }

        [Parameter]
        public string CssClass { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public bool LoadingStateDisabled { get; set; } = false;

        [Parameter]
        public HtmlButtonType HtmlButtonType { get; set; } = HtmlButtonType.Button;

        [Parameter]
        public bool RenderSpinnerOnLoad { get; set; } = false;

        private string ButtonTypeClass => ButtonType.ToCssClass();

        protected override void OnInitialized()
        {
            Id ??= Guid.NewGuid().ToString();
        }

        private async Task InvokeParentMethod()
        {
            ToggleLoading();
            if (HtmlButtonType != HtmlButtonType.Submit)
            {
                await ParentMethod.InvokeAsync();
            }
            ToggleLoading();
        }

        private void ToggleLoading()
        {
            if (LoadingStateDisabled) return;
            Loading = !Loading;
        }
    }
}
