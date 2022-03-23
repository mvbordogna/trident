using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Trident.UI.Blazor.Components;

namespace Trident.UI.Blazor.Components.Modals
{
    public partial class MessageAlert : BaseRazorComponent
    {
        [Parameter] public string Text { get; set; }

        [Parameter] public string NavigateTo { get; set; }

        // Basic Lifecycle Functions
        //      There are Async versions of these        
        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        private void OkButton_Clicked(MouseEventArgs args)
        {
            DialogService.Close();

            if (NavigateTo == null || NavigateTo == string.Empty)
                return;
            else
                NavigationManager.NavigateTo(NavigateTo);
        }


    }
}
