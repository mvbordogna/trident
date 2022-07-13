using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Radzen;
using Trident.Contracts.Api.Client;
using Trident.UI.Blazor.Components.Modals;
using Trident.UI.Blazor.Logging.AppInsights;
using Trident.UI.Blazor.Models;
using Trident.UI.Client.Contracts.Models;

namespace Trident.UI.Blazor.Components
{
    public partial class BaseRazorComponent : ComponentBase, IDisposable
    {
        [CascadingParameter(Name = "Application")]
        public ApplicationContext CascadedApplication
        {
            get => Application;
            set => Application = value;
        }

        [Parameter]
        public ApplicationContext Application { get; set; }

        
        [Inject]
        public DialogService DialogService { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ILogger<BaseRazorComponent> BaseLogger { get; set; }

        public readonly int MobileScreenWidth = 576;

        //public async Task<ApplicationContext> GetApplicationContext()
        //{
        //    var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        //    if (state?.User?.Identity?.IsAuthenticated ?? false)
        //    {
        //        var acctIdClaim = state.User.Claims.First(x => x.Type == "sub");

        //        if (Guid.TryParse(acctIdClaim.Value, out var acctId))
        //        {
        //            var id = acctIdClaim.Value.Replace("'", string.Empty);
        //            await ApplicationInsights.SetAuthenticatedUserContext(id);

        //            var claimsPrincipal = state.User;
        //            //var claim = claimsPrincipal.FindFirst(ClaimConstants.NameIdentifierId);
        //            var currentId = new Guid();
                    
        //            var application = new ApplicationContext
        //            {
        //                User = new UserContext(currentId, claimsPrincipal)
        //            };

        //            return application;
        //            // await InvokeAsync(this.StateHasChanged);
        //        }
        //    }
        //    else
        //    {
        //        await ApplicationInsights.ClearAuthenticatedUserContext();
        //        //await InvokeAsync(this.StateHasChanged);
        //    }

        //    return null;
        //}

        public async Task<dynamic> ShowMessage(DialogMessage message)
        {
            return await DialogService.OpenAsync<MessageAlert>(message.Title, new Dictionary<string, object>()
                {
                    { nameof(MessageAlert.Text), message.Text },
                    { nameof(MessageAlert.NavigateTo), message.NavigateTo },
                    { nameof(Application), Application }
                },
                new DialogOptions() { Width = "50%", ShowClose = false }
            );
        }


        public async Task<dynamic> ShowDialog<T>(string title, Dictionary<string, object> parameters = null, bool showClose = false)
            where T : ComponentBase
        {
            parameters ??= new Dictionary<string, object>();
            parameters["Application"] = Application;
           
            return await DialogService.OpenAsync<T>(title, parameters,
                 new DialogOptions() { ShowClose = showClose, Style = "min-height:auto;min-width:auto;width:auto;" }
             );
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns>Returns true if an Error Response was handled, Otherwise false.</returns>
        public async Task<bool> ErrorHandled<T>(Response<T> response)
            where T : class
        {
            if (!response.IsSuccessStatusCode)
            {
                if (!string.IsNullOrWhiteSpace(response.ValidationSummary))
                {
                    await ShowValidationSummary(response.ValidationSummary);
                }
                else if (response.Exception != null)
                {
                    await ShowException(response.Exception);
                }
            }

            return !response.IsSuccessStatusCode;
        }

        public async Task ShowException(Exception ex)
        {
            await ShowMessage(new DialogMessage() { Title = "Error Encountered", Text = ex.Message });
        }


        public async Task ShowValidationSummary(string text)
        {
            await ShowMessage(new DialogMessage() { Title = "Validation Error", Text = text });
        }

        public async Task ShowMessage(string title, string text)
        {
            await ShowMessage(new DialogMessage() { Title = title, Text = text });
        }

        public void HandleException(Exception e, string componentName, string message)
        {
            BaseLogger.LogError(e, $"{componentName} - {message}");
            var encodedUrl = HttpUtility.UrlEncode(NavigationManager.ToBaseRelativePath(NavigationManager.Uri));
            NavigationManager.NavigateTo($"/ErrorPage/{encodedUrl}");
        }

        public virtual void Dispose() { }
    }
}
