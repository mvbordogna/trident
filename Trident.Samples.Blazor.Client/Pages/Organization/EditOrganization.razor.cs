using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident.Sample.UI.ViewModels;
using Trident.Samples.Contracts.Models;
using Trident.Samples.Contracts.Services;
using Trident.UI.Blazor.Components.Modals;
using Trident.UI.Blazor.Models;

namespace Trident.Samples.Blazor.Client.Pages.Organization
{
    public partial class EditOrganization : ComponentBase
    {
        [Parameter]
        public ApplicationContext Application { get; set; }

        [Inject]
        private ILogger<EditOrganization> Logger { get; set; }

        [Inject]
        public DialogService DialogService { get; set; }

        [Inject]
        private IOrganizationService organizationService { get; set; }

        [Parameter]
        public OrganizationModel OrganizationModel { get; set; }

        private bool _exceptionOccured = false;
        private OrganizationModel organization { get; set; }
        private List<Guid> SelectedDepts { get; set; } = new List<Guid>();

        private bool DisableSubmitButton = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                organization = await organizationService.GetOrganizationById(OrganizationModel.Id.ToString());
                if (organization == null)
                {
                    organization = OrganizationModel;
                }

                
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error occurred when gathering data for Organization Page");
                DisplayException();
            }
        }

        private void DisplayException()
        {
            _exceptionOccured = true;
            StateHasChanged();
        }

        
        private async Task SaveButton_Clicked()
        {
            var response = await organizationService.CreateOrganization(organization);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                DialogService.Close(organization);
            }
            else
            {
                var json = JsonConvert.SerializeObject(response.ValidationErrors);
                DialogService.Open<ErrorAlert>("Error", new Dictionary<string, object>
                {
                    {nameof(ErrorAlert.Response), json},
                    {nameof(Application), Application}
                });
            }
        }
        private OrganizationViewModel _organizationViewModel= new();
        private Task CancelButton_Clicked()
        {
            DialogService.Close(organization);
            return Task.CompletedTask;
        }
        public bool IsDepartmentSelected(DepartmentModel dept)
        {
            return SelectedDepts != null && SelectedDepts.Any(x => x == dept.OrganisationId);
        }

        public void CheckboxClicked(DepartmentModel option, object value)
        {
            var isChecked = value is bool ? (bool)value : false;
            if (isChecked)
            {
                if (!SelectedDepts.Any(x => x == option.OrganisationId))
                {
                    SelectedDepts.Add(option.OrganisationId);
                }
            }
            else
            {
                if (SelectedDepts.Any(x => x == option.OrganisationId))
                {
                    SelectedDepts.Remove(option.OrganisationId);
                }
            }

            DisableSubmitButton = _organizationViewModel.Departments.Any(x => !IsDepartmentSelected(x));
            StateHasChanged();
        }

        public async Task SubmitButtonClick()
        {
            //var upd = await SchoolService.UpdateTermsAndConditions(SchoolProfile.Id, SelectedTerms);
            //upd.ProfileCompleted = true;
            //upd.ProfileCompletedOn = DateTime.UtcNow;

            //SelectedTerms = upd.Terms;
            //var result = await SchoolService.Update(upd);
            //if (!result.IsSuccessStatusCode)
            //{
            //    await Application.ShowValidationSummary(result.ValidationSummary ?? result.StatusCode.ToString());
            //}
        }
    }
}
