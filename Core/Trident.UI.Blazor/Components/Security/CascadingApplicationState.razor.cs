//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.JSInterop;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Trident.UI.Blazor.Models;

//namespace Trident.UI.Blazor.Components.Security;

//public partial class CascadingApplicationState : ComponentBase, IDisposable
//{
//    #region Parameters

//    [Parameter]
//    public RenderFragment ChildContent { get; set; }

//    #endregion

//    #region Services

//    [Inject]
//    private IJSRuntime JsRuntime { get; set; }

//    [Inject]
//    private NavigationManager NavigationManager { get; set; }

//    [Inject]
//    private ILocalize Localize { get; set; }

//    [Inject]
//    private IUserLocalizer UserLocalizer { get; set; }

//    [Inject]

//    private IAccountService AccountService { get; set; }

//    [Inject]
//    private ITenantConfigurationService TenantConfigurationService { get; set; }

//    [Inject]
//    private IStateService StateService { get; set; }

//    [Inject]
//    private IMaritalStatusService MaritalStatusService { get; set; }

//    [Inject]
//    private IGenderService GenderService { get; set; }

//    [Inject]
//    private ILanguageService LanguageService { get; set; }

//    [Inject]
//    private IEthnicityService EthnicityService { get; set; }

//    [Inject]
//    private IGradeLevelService GradeLevelService { get; set; }

//    [Inject]
//    private IPhoneTypeService PhoneTypeService { get; set; }

//    [Inject]
//    private ISuffixService SuffixService { get; set; }

//    [Inject]
//    private IRaceService RaceService { get; set; }

//    [Inject]
//    private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

//    [Inject]
//    private IApplicationInsights ApplicationInsights { get; set; }

//    [Inject]
//    private IPersonService PersonService { get; set; }

//    [Inject]
//    private IStudentService StudentService { get; set; }

//    [Inject]
//    private INavigationConfigurationService NavigationConfigurationService { get; set; }

//    #endregion

//    private ApplicationContext ApplicationContext { get; set; } = new();
//    private Task<AuthenticationState> _currentAuthenticationStateTask;
//    private bool _applicationDataLoaded = false;

//    protected override async Task OnInitializedAsync()
//    {
//        _currentAuthenticationStateTask ??= AuthenticationStateProvider.GetAuthenticationStateAsync();

//        var state = await _currentAuthenticationStateTask;
//        var authenticated = state?.User?.Identity?.IsAuthenticated;
//        if (authenticated == true)
//        {
//            await RefreshUserContext();
//            await InvokeAsync(StateHasChanged);
//        }
//        else
//        {
//            AuthenticationStateProvider.AuthenticationStateChanged +=
//                async (authenticationStateTask) => await AuthenticationStateChanged(authenticationStateTask);
//        }

//        await base.OnInitializedAsync();
//    }

//    private async Task AuthenticationStateChanged(Task<AuthenticationState> authenticationStateTask)
//    {
//        _currentAuthenticationStateTask = authenticationStateTask;
//        var state = await _currentAuthenticationStateTask;
//        var authenticated = state?.User?.Identity?.IsAuthenticated;
//        if (authenticated != true)
//        {
//            return;
//        }

//        await RefreshUserContext();
//        await InvokeAsync(StateHasChanged);
//    }

//    void IDisposable.Dispose()
//    {
//        AuthenticationStateProvider.AuthenticationStateChanged -= async (authenticationStateTask) => await AuthenticationStateChanged(authenticationStateTask);
//    }

//    private async Task RefreshUserContext()
//    {
//        var state = await _currentAuthenticationStateTask;
//        if (state?.User?.Identity?.IsAuthenticated ?? false)
//        {
//            var acctIdClaim = state.User.Claims.First(x => x.Type == "sub");

//            if (Guid.TryParse(acctIdClaim.Value, out var acctId))
//            {
//                if (ApplicationContext.User?.UserId != acctId)
//                {
//                    var id = acctIdClaim.Value.Replace("'", string.Empty);
//                    await ApplicationInsights.SetAuthenticatedUserContext(id);

//                    var claimsPrincipal = state.User;
//                    var claim = claimsPrincipal.FindFirst(ClaimConstants.NameIdentifierId);
//                    var currentId = new Guid(id);                    
                    
//                    ApplicationContext.User = new UserContext(currentId, claimsPrincipal);
//                    ApplicationContext.Lookups = await LoadLookupsAsync<T>();

//                    _applicationDataLoaded = true;
//                }
//            }
//        }
//        else

//        {
//            await ApplicationInsights.ClearAuthenticatedUserContext();
//        }
//    }

//    protected virtual async Task<T> LoadLookupsAsync<T>() where T:LookupContext
//    {
//        var lookupContext = new LookupContext();
        
//        return (T)lookupContext;
//    }
//}
