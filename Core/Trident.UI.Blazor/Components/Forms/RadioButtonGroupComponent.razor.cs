using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trident.UI.Blazor.Components.Forms
{
    public partial class RadioButtonGroupComponent
    {
        [Parameter]
        public bool Inline { get; set; } = true;

        [Parameter]
        public List<RadioButtonModel> RadioButtonOptions { get; set; }

        [Parameter]
        public EventCallback<string> OnChangeMethod { get; set; }

        [Parameter]
        public string SelectedValue { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        private async Task UpdateCurrentValue(string value)
        {
            await OnChangeMethod.InvokeAsync(value);
        }
    }
}
