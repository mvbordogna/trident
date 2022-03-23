using System.Threading.Tasks;
using Trident.Api.Search;

namespace Trident.UI.Blazor.Components.Grid.Filters
{
    public abstract class TextboxFilter : FilterComponentBase<TextboxFilterOptions, FilterEventArgs<TextboxFilter>>
    {
    }

    public partial class TextboxFilter<TOutputValue> : TextboxFilter
    {

        /// <summary>
        ///  // get init config data here for list loading
        /// </summary>
        /// <returns></returns>
        protected override Task OnParametersSetAsync()
        {

            return base.OnParametersSetAsync();
        }

        /// <summary>
        /// Validates that the filter value is of the matching column type
        /// </summary>
        /// <returns></returns>
        public override Task<bool> IsValid()
        {
            try
            {
                if (Value == null) return Task.FromResult(true);
                var specificedTypeValue = Value.ChangeType(DataType);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Apply filter data to the criteria
        /// </summary>
        /// <param name="searchCriteria"></param>
        public override void ApplyFilter(SearchCriteriaModel searchCriteria)
        {
            base.ApplyFilter(searchCriteria);
        }

        /// <summary>
        /// Implment change event for your congtrol types to collect the values as they
        /// are changing..only if valid do we raise the event to the parent grid panel
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public async Task ValueChanged(string newValue)
        {
            var oldValue = Value;
            Value = newValue;
            await RaiseOnChange(oldValue, Value);
        }

        private async Task RaiseOnChange(object oldValue, object newValue)
        {
            if (OnChange.HasDelegate && await IsValid())
            {
                await OnChange.InvokeAsync(new FilterEventArgs<TextboxFilter>(this)
                {
                    OriginalValue = oldValue,
                    NewValue = newValue

                });
            }
        }


        protected override async Task ResetToDefault()
        {
            var oldValue = Value;
            Value = FilterOptions.DefaultValue;
            await RaiseOnChange(oldValue, Value);
        }
    }
}
