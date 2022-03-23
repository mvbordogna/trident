using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident.Api.Search;
using Trident.Search;

namespace Trident.UI.Blazor.Components.Grid.Filters
{
    public abstract class DateFilter : FilterComponentBase<DatetimeFilterOptions, FilterEventArgs<DateFilter>> { }

    public partial class DateFilter<TOutputValue> : DateFilter
    {
        [Parameter]
        public string FormatMask { get; set; } = "MM/dd/yyyy";
        public DateTime? LowerBoundValue { get; set; }
        public DateTime? UpperBoundValue { get; set; }

        private static Type[] SupportedTypeParameters { get; } = new[] {
                   typeof(DateTime),
                   typeof(DateTimeOffset),
                   typeof(DateTime?),
                   typeof(DateTimeOffset?)
               };

        public DateFilter()
        {
            if (!SupportedTypeParameters.Any(x => x == typeof(TOutputValue)))
                throw new ArgumentOutOfRangeException(
                    $"Only the following types are supported as the {string.Join(", ", SupportedTypeParameters.Select(x => x.FullName))}");
        }


        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            FormatMask ??= FilterOptions?.FormatMask;
        }

        private async Task ValueChanged(Bounds bound, DateTime? newValue)
        {
            DateTimeOffset? origValue;
            if (bound == Bounds.Upper)
            {
                origValue = UpperBoundValue;
                UpperBoundValue = newValue;
            }
            else
            {
                LowerBoundValue = newValue;
                origValue = LowerBoundValue;
            }

            await RaiseOnChangeEvent(origValue, newValue, bound);

        }

        private async Task RaiseOnChangeEvent(object origValue, object newValue, Bounds bound)
        {
            if (OnChange.HasDelegate && await IsValid())
            {
                await OnChange.InvokeAsync(new DatetimeFilterArgs(this)
                {
                    OriginalValue = origValue,
                    NewValue = newValue,
                    Bound = bound

                }); ;
            }
        }

        public override Task<bool> IsValid()
        {
            var isValid = !(LowerBoundValue > UpperBoundValue);
            return Task.FromResult(isValid);
        }

        protected override async Task ResetToDefault()
        {
            var origValueLower = LowerBoundValue;
            var origValueUpper = UpperBoundValue;

            LowerBoundValue = FilterOptions.LowerBoundDefaultValue.HasValue ? FilterOptions.LowerBoundDefaultValue.GetValueOrDefault().DateTime : null;
            await RaiseOnChangeEvent(origValueLower, LowerBoundValue, Bounds.Lower);

            UpperBoundValue = FilterOptions.UpperBoundDefaultValue.HasValue ? FilterOptions.UpperBoundDefaultValue.GetValueOrDefault().DateTime : null;
            await RaiseOnChangeEvent(origValueUpper, UpperBoundValue, Bounds.Upper);
        }

        private object GetSettingsBasedDateTimeValue(DateTimeOffset value)
        {
            if (typeof(TOutputValue) == typeof(DateTime)
                || typeof(TOutputValue) == typeof(DateTime?))
                return value.DateTime;
            else
                return value;
        }

        public override void ApplyFilter(SearchCriteriaModel searchCriteria)
        {
            var hasLowerBound = LowerBoundValue.HasValue;
            var hasUpperBound = UpperBoundValue.HasValue;
            int keyIdx = 1;
            string keyBase = FilterOptions.FilterPath;

            if (!hasLowerBound && !hasUpperBound)
            {
                if (searchCriteria.Filters.ContainsKey(FilterOptions.FilterPath))
                {
                    searchCriteria.Filters.Remove(FilterOptions.FilterPath);
                }

                return;
            }

            IJunction query = AxiomFilterBuilder
                .CreateFilter()
                .StartGroup();

            // lower only 
            if (hasLowerBound)
            {
                query = ((GroupStart)query).AddAxiom(new Axiom
                {
                    Field = FilterOptions.FilterPath,
                    Key = $"{keyBase}{keyIdx++}",
                    Operator = Search.CompareOperators.gte,
                    Value = GetSettingsBasedDateTimeValue(LowerBoundValue.Value)
                });
            }

            if (hasLowerBound && hasUpperBound)
            {
                query = ((AxiomTokenizer)query).And();
            }

            if (hasUpperBound)
            {
                var upper = UpperBoundValue.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
                if (query is ILogicalOperator and)
                {
                    query = and.AddAxiom(new Axiom
                    {
                        Field = FilterOptions.FilterPath,
                        Key = $"{keyBase}{keyIdx++}",
                        Operator = Search.CompareOperators.lt,
                        Value = GetSettingsBasedDateTimeValue(upper)
                    });
                }
                else
                {
                    query = ((GroupStart)query).AddAxiom(new Axiom
                    {
                        Field = FilterOptions.FilterPath,
                        Key = $"{keyBase}{keyIdx++}",
                        Operator = Search.CompareOperators.gte,
                        Value = GetSettingsBasedDateTimeValue(upper)
                    });
                }
            }

            var filter = ((AxiomTokenizer)query)
                .EndGroup()
                .Build();

            if (filter != null)
            {
                searchCriteria.Filters ??= new Dictionary<string, object>();
                searchCriteria.Filters[FilterOptions.FilterPath] = filter;
            }
        }


    }
}
