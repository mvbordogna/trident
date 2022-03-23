using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System.Threading.Tasks;
using Trident.UI.Blazor.Components;

namespace Trident.UI.Blazor.Components.Modals
{
    public partial class ErrorAlert : BaseRazorComponent
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public string Response { get; set; }

        // Basic Lifecycle Functions
        //      There are Async versions of these        
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Title = "Error Alert";
        }

        private Task OkButton_Clicked(MouseEventArgs args)
        {
            DialogService.Close();
            return Task.CompletedTask;
        }
    }
}
