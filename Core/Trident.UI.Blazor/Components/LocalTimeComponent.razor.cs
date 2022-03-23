using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Trident.UI.Blazor.Contracts.Services;

namespace Trident.UI.Blazor.Components
{
    public partial class LocalTimeComponent : ComponentBase
    {
        [Inject]
        private ITimeZoneService TimeZoneService { get; set; }

        [Parameter]
        public DateTimeOffset UtcInput { get; set; }

        [Parameter]
        public string Format { get; set; } = "MM/dd/yyyy hh:mm:ss";

        private string _output = string.Empty;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (TimeZoneInfo.ConvertTimeToUtc(UtcInput.DateTime) == TimeZoneInfo.ConvertTimeToUtc(DateTime.MinValue))
            {
                _output = "";
            }
            else
            {
                UtcInput = await TimeZoneService.GetLocalDateTime(UtcInput);
                _output = UtcInput.ToString(Format);
            }
        }
    }
}
