using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trident.Api.Search;
using Trident.Contracts.Enums;
using Trident.Samples.Contracts.Models;
using Trident.Samples.Contracts.Services;

namespace Trident.Samples.Blazor.Client.Pages.Organization
{

    public partial class OrganizationGrid : ComponentBase, IDisposable
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IOrganizationService organizationService{ get; set; }

        [Inject]
        private ILogger<OrganizationGrid> Logger { get; set; }

        private bool _exceptionOccured = false;
        private IEnumerable<OrganizationModel> _results { get; set; } = new List<OrganizationModel>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {                
                await LoadData();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error occurred when gathering data for Organization List Page");
                DisplayException();
            }

        }

        private async Task LoadData()
        {
            _results = await organizationService.GetAll();
        }


        private async void Close(dynamic result)
        {
            await LoadData();
        }

        private void NavigateToNewOrganization()
        {
            NavigationManager.NavigateTo("/Organization/New");
        }
        private void DisplayException()
        {
            _exceptionOccured = true;
            StateHasChanged();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
