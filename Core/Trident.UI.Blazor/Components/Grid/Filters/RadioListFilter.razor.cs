using System.Threading.Tasks;
using Trident.Api.Search;

namespace Trident.UI.Blazor.Components.Grid.Filters
{
    public abstract class RadioListFilter : ListFilterComponentBase<RadioButtonFilterOptions, FilterEventArgs<RadioListFilter>>
    {
    }

    public partial class RadioListFilter<TOutputValue> : RadioListFilter
    {
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

        public new TOutputValue Value
        {
            get => base.Value != null ? (TOutputValue)base.Value : default;
            set => base.Value = value;
        }

        protected override async Task ResetToDefault()
        {
            var origValue = Value;
            Value = default;
            await RaiseOnChange(origValue, Value);
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
        public async Task OnSelectionChanged(TOutputValue newValue)
        {
            var oldValue = Value;
            Value = newValue;
            await RaiseOnChange(oldValue, newValue);
        }


        public async Task RaiseOnChange(object oldValue, object newValue)
        {
            if (OnChange.HasDelegate && await IsValid())
            {
                await OnChange.InvokeAsync(new FilterEventArgs<RadioListFilter>(this)
                {
                    OriginalValue = oldValue,
                    NewValue = Value
                });
            }
        }


    }
}
