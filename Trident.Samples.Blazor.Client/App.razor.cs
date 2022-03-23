using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Trident.Samples.Blazor.Client
{
    public partial class App
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private ILogger<App> Logger { get; set; }

        private async Task RouterNavigating(NavigationContext navigationContext)
        {
            Logger.LogInformation(navigationContext.Path);
        }
    }
}
