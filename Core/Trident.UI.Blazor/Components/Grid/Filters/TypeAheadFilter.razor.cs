using Radzen;
using System.Threading.Tasks;
using Trident.Api.Search;

namespace Trident.UI.Blazor.Components.Grid.Filters
{
    public abstract class TypeAheadFilter : ListFilterComponentBase<TypeAheadFilterOptions, FilterEventArgs<TypeAheadFilter>>
    {
    }

    public partial class TypeAheadFilter<TOutputValue> : TypeAheadFilter
    {
        public TypeAheadFilter()
        {
            DisableInitLoad = true;
        }

        public override Task<bool> IsValid()
        {
            try
            {
                if (Value == null) return Task.FromResult(true);
                Value.ChangeType(DataType);
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

        // <summary>
        // Implement change event for your control-types to collect the values as they are changing.
        // We only raise the event to the parent grid panel if valid.
        // </summary>
        // <param name="newValue"></param>
        // <returns></returns>
        public async Task OnSelectionChanged(object newValue)
        {
            var oldValue = Value;
            Value = (TOutputValue)newValue;
            await RaiseOnChange(oldValue, Value);
        }

        private async Task RaiseOnChange(object oldValue, object newValue)
        {
            if (OnChange.HasDelegate && await IsValid())
            {
                await OnChange.InvokeAsync(new FilterEventArgs<TypeAheadFilter>(this)
                {
                    OriginalValue = oldValue,
                    NewValue = Value
                });
            }
        }

        private async Task OnLoadData(LoadDataArgs args)
        {
            ListData = await GetOptions(args.Filter);
        }

        protected override async Task ResetToDefault()
        {
            var oldValue = Value;
            Value = FilterOptions.DefaultValue;
            await RaiseOnChange(oldValue, Value);
        }
    }
}
