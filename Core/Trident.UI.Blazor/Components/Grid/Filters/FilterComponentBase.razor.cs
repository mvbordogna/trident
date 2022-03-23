using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Trident.Api.Search;

namespace Trident.UI.Blazor.Components.Grid.Filters
{
    public interface IFilterCompoent
    {
        GridFilterPanel FilterPanel { get; set; }
        FilterOptions FilterOptions { get; set; }
        object Value { get; set; }
        object OnChange { get; }
        void ApplyFilter(SearchCriteriaModel searchCriteria);
        Guid InstanceId { get; }
        Type DataType { get; set; }
        Task Reset(SearchCriteriaModel criteria);
        Task<bool> IsValid();
    }

    public abstract partial class FilterComponentBase<TOptions, TEventArgs> : BaseRazorComponent, IFilterCompoent
        where TOptions : FilterOptions
        where TEventArgs : FilterEventArgs
    {
        [Parameter]
        public GridFilterPanel FilterPanel { get; set; }

        [CascadingParameter(Name = nameof(FilterPanel))]
        public GridFilterPanel CascadedFilterPanel
        {
            get { return FilterPanel; }
            set { FilterPanel = value; }
        }

        [Parameter]
        public TOptions FilterOptions { get; set; }

        [Parameter]
        public EventCallback<TEventArgs> OnChange { get; set; }

        [Parameter]
        public object Value { get; set; }

        [Parameter]
        public Type DataType { get; set; }

        FilterOptions IFilterCompoent.FilterOptions
        {
            get => FilterOptions;
            set => FilterOptions = value as TOptions;
        }

        object IFilterCompoent.OnChange { get => OnChange; }

        public Guid InstanceId { get; } = Guid.NewGuid();

        public abstract Task<bool> IsValid();

        protected abstract Task ResetToDefault();

        public async Task Reset(SearchCriteriaModel criteria)
        {
            await ResetToDefault();
            ApplyFilter(criteria);
        }


        public virtual void ApplyFilter(SearchCriteriaModel searchCriteria)
        {
            if (searchCriteria != null)
            {
                if (Value == null)
                {
                    if (searchCriteria.Filters.ContainsKey(FilterOptions.FilterPath))
                    {
                        searchCriteria.Filters.Remove(FilterOptions.FilterPath);
                    }
                }
                else
                {
                    searchCriteria.Filters[FilterOptions.FilterPath] = new CompareModel()
                    {
                        Operator = FilterOptions.Operator,
                        Value = Value
                    };
                }
            }
        }

        protected override void OnInitialized()
        {
            FilterPanel.ActiveFilters.Add(this);
        }
    }
}
