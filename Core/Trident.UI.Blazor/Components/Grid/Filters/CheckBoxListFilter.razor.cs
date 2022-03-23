using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident;
using Trident.Api.Search;
using Trident.Search;

namespace Trident.UI.Blazor.Components.Grid.Filters;

public abstract class CheckBoxListFilter : ListFilterComponentBase<CheckboxFilterOptions, FilterEventArgs<CheckBoxListFilter>> { }

public partial class CheckBoxListFilter<TOutputValue> : CheckBoxListFilter
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

            if (Value is IEnumerable<TOutputValue> values)
            {
                foreach (var v in values)
                {
                    v.ChangeType(DataType);
                }
            }

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public new IEnumerable<TOutputValue> Value
    {
        get => base.Value != null ? (IEnumerable<TOutputValue>)base.Value : null;
        set => base.Value = value;
    }

    /// <summary>
    /// Apply filter data to the criteria
    /// </summary>
    /// <param name="searchCriteria"></param>
    public override void ApplyFilter(SearchCriteriaModel searchCriteria)
    {
        var values = Value as IEnumerable<TOutputValue>;

        if (values == null || !values.Any())
        {
            if (searchCriteria.Filters.ContainsKey(FilterOptions.FilterPath))
            {
                searchCriteria.Filters.Remove(FilterOptions.FilterPath);
            }
            return;
        }

        IJunction query = AxiomFilterBuilder.CreateFilter()
            .StartGroup();
        var i = 0;
        foreach (var v in values)
        {
            var selection = v.ChangeType(DataType);
            if (query is GroupStart groupStart)
            {
                query = groupStart.AddAxiom(new Axiom()
                {
                    Key = $"{FilterOptions.FilterPath}_{i++}",
                    Field = FilterOptions.FilterPath,
                    Operator = Search.CompareOperators.eq,
                    Value = v
                });

            }
            else if (query is AxiomTokenizer axiom)
            {
                query = axiom.Or().AddAxiom(new Axiom()
                {
                    Key = $"{FilterOptions.FilterPath}_{i++}",
                    Field = FilterOptions.FilterPath,
                    Operator = Search.CompareOperators.eq,
                    Value = v
                });
            }

            searchCriteria.Filters[FilterOptions.FilterPath] = (query as AxiomTokenizer)?.EndGroup().Build();
        }
    }

    /// <summary>
    /// Implment change event for your congtrol types to collect the values as they
    /// are changing..only if valid do we raise the event to the parent grid panel
    /// </summary>
    /// <param name="newValue"></param>
    /// <returns></returns>
    public async Task OnSelectionChanged(IEnumerable<TOutputValue> newValue)
    {
        var oldValue = Value;
        Value = newValue;
        await RaisOnChangeEvent(oldValue, newValue);
    }
    protected override async Task ResetToDefault()
    {
        var oldValue = Value;
        Value = new List<TOutputValue>();
        await RaisOnChangeEvent(oldValue, Value);
    }


    private async Task RaisOnChangeEvent(object oldValue, object newValue)
    {
        if (OnChange.HasDelegate && await IsValid())
        {
            await OnChange.InvokeAsync(new FilterEventArgs<CheckBoxListFilter>(this)
            {
                OriginalValue = oldValue,
                NewValue = Value
            });
        }
    }

}
