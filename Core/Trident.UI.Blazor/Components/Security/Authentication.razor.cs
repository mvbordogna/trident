using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;


namespace Trident.UI.Blazor.Components.Security
{
    public class ApplicationAuthenticationState : RemoteAuthenticationState
    {
        public string Id { get; set; }
    }


    public class StateContainer
    {
        public int CounterValue { get; set; }

        public string GetStateForLocalStorage()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void SetStateFromLocalStorage(string locallyStoredState)
        {
            var deserializedState = JsonConvert.DeserializeObject<StateContainer>(locallyStoredState);
            CounterValue = deserializedState.CounterValue;
        }
    }

    public partial class Authentication : ComponentBase
    {
        [Parameter]
        public string Action { get; set; }


        [Inject]
        private IJSRuntime JS { get; set; }
        [Inject]
        private StateContainer State { get; set; }
        [Inject]
        private ILogger<Authentication> Logger { get; set; }

        public ApplicationAuthenticationState AuthenticationState { get; set; } =
            new ApplicationAuthenticationState();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (RemoteAuthenticationActions.IsAction(RemoteAuthenticationActions.LogIn,
                        Action) ||
                    RemoteAuthenticationActions.IsAction(RemoteAuthenticationActions.LogOut,
                        Action))
                {
                    AuthenticationState.Id = Guid.NewGuid().ToString();

                    await JS.InvokeVoidAsync("sessionStorage.setItem",
                        AuthenticationState.Id, State.GetStateForLocalStorage());
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An exception occurred in Authentication.OnInitializedAsync");
            }
        }

        private async Task RestoreState(ApplicationAuthenticationState state)
        {
            try
            {
                if (state.Id != null)
                {
                    var locallyStoredState = await JS.InvokeAsync<string>(
                        "sessionStorage.getItem", state.Id);

                    if (locallyStoredState != null)
                    {
                        State.SetStateFromLocalStorage(locallyStoredState);
                        await JS.InvokeVoidAsync("sessionStorage.removeItem", state.Id);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An exception occurred in the Authentication.RestoreState method");
            }
        }
    }
}
