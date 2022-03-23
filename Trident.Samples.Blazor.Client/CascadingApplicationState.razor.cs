//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.JSInterop;

//namespace Trident.Samples.Blazor.Client;

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
//                    var account = await AccountService.GetAccountByLocalAuthId(id);
//                    Person person = null;
//                    List<StudentModel> studentList = null;
//                    NavigationModel navConfig = null;
//                    TenantConfigurationModel tenantConfig = null;

//                    var tasks = new List<Task>();
//                    tasks.Add(
//                        Task.Run(async () =>
//                        {
//                            person = await PersonService.GetById(new Guid(account.PersonId), account.AccountType);
//                            UserLocalizer.SetUser(currentId, person);
//                            await Localize.Initialize();
//                        }));

//                    if (account.AccountType == "Guardian")
//                    {
//                        tasks.Add(
//                            Task.Run(async () =>
//                            {
//                                studentList = await StudentService.GetStudentSelectListByGuardianId(new Guid(account.PersonId));
//                            }));
//                    }

//                    tasks.Add(Task.Run(async () =>
//                    {
//                        navConfig = await NavigationConfigurationService.GetAuthFilteredNavigationConfiguration("SUFS", claimsPrincipal);
//                    }));

//                    tasks.Add(Task.Run(async () =>
//                    {
//                        tenantConfig = await TenantConfigurationService.GetActiveTenantAsync();
//                    }));

//                    await Task.WhenAll(tasks);
                        
//                    if (account.AccountType == "Guardian")
//                    {
//                        var filterNav = new List<NavigationItemModel>();
//                        foreach (var item in navConfig.NavigationItems)
//                        {
//                            if ((item.ClaimType == Claims.Marketplace
//                                || item.ClaimType == Claims.GuardianReimbursementMenu)
//                                && !studentList.Any())
//                            {
//                                continue;
//                            }

//                            filterNav.Add(item);
//                        }
//                        navConfig.NavigationItems = filterNav;
//                    }

//                    ApplicationContext.User = new UserContext(currentId, claimsPrincipal, account, person, navConfig);
//                    ApplicationContext.Tenant = tenantConfig;
//                    ApplicationContext.Lookups = await LoadLookupsAsync();

//                    _applicationDataLoaded = true;
//                }
//            }
//        }
//        else

//        {
//            await ApplicationInsights.ClearAuthenticatedUserContext();
//        }
//    }

//    private async Task<LookupContext> LoadLookupsAsync()
//    {
//        var lookupContext = new LookupContext();
//        await Task.WhenAll(
//            Task.Run(async () =>
//            {
//                var genderOptions = (await GenderService.GetEnabled()).ToList();
//                genderOptions.ForEach(x => x.Name = UserLocalizer.GetText(x.Name));
//                lookupContext.GenderOptions = genderOptions;
//            }),
//            Task.Run(async () =>
//            {
//                var raceOptions = (await RaceService.GetAll()).ToList();
//                raceOptions.ForEach(x => x.Name = UserLocalizer.GetText(x.Name));
//                lookupContext.RaceOptions = raceOptions;
//            }),
//            Task.Run(async () =>
//            {
//                var ethnicityOptions = (await EthnicityService.GetAll()).ToList();
//                ethnicityOptions.ForEach(x => x.Name = UserLocalizer.GetText(x.Name));
//                lookupContext.EthnicityOptions = ethnicityOptions;
//            }),
//            Task.Run(async () =>
//            {
//                var suffixOptions = (await SuffixService.GetAll()).ToList();
//                suffixOptions.ForEach(x => x.Name = UserLocalizer.GetText(x.Name));
//                lookupContext.SuffixOptions = suffixOptions;
//            }),
//            Task.Run(async () => { lookupContext.GradeLevelOptions = (await GradeLevelService.GetAll()).OrderBy(o => o.Name).ToList(); }),
//            Task.Run(async () => { lookupContext.StateOptions = (await StateService.GetAll()).ToList(); }),
//            Task.Run(async () => { lookupContext.LanguageOptions = (await LanguageService.GetAll()).OrderBy(o => o.Name).ToList(); }),
//            Task.Run(async () =>
//            {
//                var martitialStatusOptions = (await MaritalStatusService.GetAll()).ToList();
//                martitialStatusOptions.ForEach(x => x.Name = UserLocalizer.GetText(x.Name));
//                lookupContext.MaritalStatusOptions = martitialStatusOptions;
//            }),
//            Task.Run(async () =>
//            {
//                var phoneTypeOptions = (await PhoneTypeService.GetEnabled()).ToList();
//                phoneTypeOptions.ForEach(x => x.Name = UserLocalizer.GetText(x.Name));
//                lookupContext.PhoneTypeOptions = phoneTypeOptions;
//            })
//        );
//        return lookupContext;
//    }
//}
