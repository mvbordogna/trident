using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident.Api.Search;
using Trident.UI.Blazor.Components;
using Trident.UI.Blazor.Components.Grid.Filters;

namespace Trident.UI.Blazor.Components.Grid;

public class GridFilter
{
    public string FilterDisplayName { get; set; }
    public object FilterOptions { get; set; }
    public Type FilterType { get; set; }
    public int FilterGroupNumber { get; set; }
    public Type DataType { get; set; }
    public int ColumnOrder { get; internal set; }
    public object InitialValue { get; internal set; }
}

public abstract class GridFilterPanel : BaseRazorComponent
{
    internal List<IFilterCompoent> ActiveFilters { get; set; } = new List<IFilterCompoent>();
}

public partial class GridFilterPanel<TItem> : GridFilterPanel
{
    [Inject]
    private IComponentGenerator ComponentGenerator { get; set; }

    [Parameter]
    public List<ColumnDefinition<TItem>> Columns { get; set; } = new();

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public SearchResultInfoModel<SearchCriteriaModel> SearchCriteria { get; set; }

    [Parameter]
    public EventCallback<SearchResultInfoModel<SearchCriteriaModel>> CriteriaChanged { get; set; }

    private EventCallbackFactory _eventCallbackFactory = new();

    private bool _hasFilterChanges;

    private bool EnableApply { get; set; } = true;

    private List<List<RenderFragment>> _filterRenderFragments = new();

    private readonly Dictionary<string, Type> _filterTypes = new()
    {
        { nameof(CheckBoxListFilter), typeof(CheckBoxListFilter<>) },
        { nameof(DateFilter), typeof(DateFilter<>) },
        { nameof(RadioListFilter), typeof(RadioListFilter<>) },
        { nameof(TextboxFilter), typeof(TextboxFilter<>) },
        { nameof(TypeAheadFilter), typeof(TypeAheadFilter<>) }
    };

    private readonly Dictionary<string, Type> _filterOptionTypes = new()
    {
        { nameof(CheckBoxListFilter), typeof(CheckboxFilterOptions) },
        { nameof(DateFilter), typeof(DatetimeFilterOptions) },
        { nameof(RadioListFilter), typeof(RadioButtonFilterOptions) },
        { nameof(TextboxFilter), typeof(TextboxFilterOptions) },
        { nameof(TypeAheadFilter), typeof(TypeAheadFilterOptions) }
    };

    private readonly Dictionary<Type, Func<object>> _filterChangeEventFactory = new();

    private void InitEventHandlerFactory()
    {
        _filterChangeEventFactory.Add(typeof(CheckBoxListFilter<>),
            () => _eventCallbackFactory.Create<FilterEventArgs<CheckBoxListFilter>>(this, OnfilterChanged));
        _filterChangeEventFactory.Add(typeof(DateFilter<>), () => _eventCallbackFactory.Create<FilterEventArgs<DateFilter>>(this, OnfilterChanged));
        _filterChangeEventFactory.Add(typeof(RadioListFilter<>), () => _eventCallbackFactory.Create<FilterEventArgs<RadioListFilter>>(this, OnfilterChanged));
        _filterChangeEventFactory.Add(typeof(TextboxFilter<>), () => _eventCallbackFactory.Create<FilterEventArgs<TextboxFilter>>(this, OnfilterChanged));
        _filterChangeEventFactory.Add(typeof(TypeAheadFilter<>), () => _eventCallbackFactory.Create<FilterEventArgs<TypeAheadFilter>>(this, OnfilterChanged));
    }

    protected override void OnInitialized()
    {
        if (Columns == null) return;

        InitEventHandlerFactory();

        var filterData = GetFilterDataFromColumns();
        CreateFilterRenderFragments(filterData);

        Visible = true;
    }

    private void CreateFilterRenderFragments(List<IGrouping<int, GridFilter>> filterData)
    {
        var currentIndex = 0;

        foreach (var group in filterData)
        {
            _filterRenderFragments.Add(new List<RenderFragment>());

            foreach (var filter in group)
            {
                var filterType = !filter.FilterType.ContainsGenericParameters
                    ? filter.FilterType
                    : GetGenericFilterTyped(filter.FilterType, filter.DataType);

                var filterRendering = ComponentGenerator.CreateDynamicComponent(filterType, new Dictionary<string, object>()
                {
                    {nameof(Application), Application},
                    {nameof(IFilterCompoent.FilterPanel), this},
                    {nameof(IFilterCompoent.FilterOptions), filter.FilterOptions},
                    {nameof(IFilterCompoent.OnChange), _filterChangeEventFactory[filter.FilterType]()},
                    {nameof(IFilterCompoent.DataType), filter.DataType},
                });

                _filterRenderFragments[currentIndex].Add(filterRendering);
            }

            currentIndex++;
        }
    }

    private List<IGrouping<int, GridFilter>> GetFilterDataFromColumns()
    {
        var i = 0;

        var filterData = Columns
            .Where(x => !string.IsNullOrWhiteSpace(x.FilterType) && x.FilterEnabled)
            .Select(x =>
            {
                FilterOptions filterOptions = null;

                if (x.FilterOptions is JObject jObj)
                {
                    filterOptions = (FilterOptions)((JObject)x.FilterOptions).ToObject(_filterOptionTypes[x.FilterType]);
                }
                else if (x.FilterOptions is FilterOptions fo)
                {
                    filterOptions = fo;
                }

                if (filterOptions == null) return null;
                return new GridFilter()
                {
                    FilterDisplayName = x.Title,
                    FilterOptions = filterOptions,
                    FilterType = _filterTypes.ContainsKey(x.FilterType) ? _filterTypes[x.FilterType] : null,
                    FilterGroupNumber = filterOptions.FilterGroupNumber,
                    DataType = x.PropertyType,
                    ColumnOrder = i++,
                };
            })
            .Where(x => x?.FilterType != null)
            .OrderBy(x => x.ColumnOrder)
            .GroupBy(x => x.FilterGroupNumber, x => x)
            .OrderBy(x => x.Key)
            .ToList();


        return filterData;
    }

    private Type GetGenericFilterTyped(Type filterType, Type outputValueType)
    {
        return filterType.MakeGenericType(outputValueType);
    }

    public async Task OnfilterChanged(FilterEventArgs args)
    {
        args.FilterComponent.ApplyFilter(SearchCriteria);
        EnableApply = !await AreAllFiltersValid();
        _hasFilterChanges = true;
    }

    private async Task ClearAllFilters()
    {
        foreach (var activeFilter in ActiveFilters)
        {
            await activeFilter.Reset(SearchCriteria);
        }

        _hasFilterChanges = true;
        await RaiseApplyFilter();
    }

    private async Task RaiseApplyFilter()
    {
        if (_hasFilterChanges)
        {
            if (await AreAllFiltersValid())
            {
                await CriteriaChanged.InvokeAsync(SearchCriteria);
                Visible = false;
                _hasFilterChanges = false;
                if (VisibleChanged.HasDelegate)
                {
                    await VisibleChanged.InvokeAsync(Visible);
                }
            }
        }
    }

    private async Task OnApplyFilters()
    {
        await RaiseApplyFilter();
    }

    private async Task OnCloseRequested()
    {
        Visible = false;
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(Visible);
        }
    }

    private async Task<bool> AreAllFiltersValid()
    {
        var allValid = true;
        foreach (var f in ActiveFilters)
        {
            if (!(allValid &= await f.IsValid()))
            {
                return allValid;
            }
        }

        return allValid;
    }
}
