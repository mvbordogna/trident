using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using Trident.Api.Search;

namespace Trident.UI.Blazor.Components.Grid.Filters
{
    public abstract class FilterOptions
    {
        public int FilterGroupNumber { get; set; }
        public string Label { get; set; }
        public string FilterPath { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CompareOperators Operator { get; set; } = CompareOperators.eq;
    }

    public class ListFilterOptions : FilterOptions
    {
        public List<ListOption<string>> ListItems { get; set; }
        public LookupEndPoint EndPoint { get; set; }
    }

    public class TypeAheadFilterOptions : ListFilterOptions
    {
        public int MinimumCharEntiry { get; set; }
        public string DefaultValue { get; set; }
    }

    public class TextboxFilterOptions : FilterOptions
    {
        public string DefaultValue { get; set; }
    }

    public class CheckboxFilterOptions : ListFilterOptions
    {
        public string DefaultValue { get; set; }
    }

    public class RadioButtonFilterOptions : ListFilterOptions
    {
        public string DefaultValue { get; set; }
    }

    public class DatetimeFilterOptions : FilterOptions
    {
        public DateTimeOffset? LowerBoundDefaultValue { get; set; }
        public DateTimeOffset? UpperBoundDefaultValue { get; set; }
        public string FormatMask { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CompareOperators UpperBoundOperator { get; set; } = CompareOperators.lte;

        [JsonConverter(typeof(StringEnumConverter))]
        public CompareOperators LowerBoundOperator { get; set; } = CompareOperators.gte;

        [JsonConverter(typeof(StringEnumConverter))]
        public ValueBoundOptions BoundType { get; set; } = ValueBoundOptions.Both;

    }

    public enum ValueBoundOptions
    {
        Upper,
        Lower,
        Both
    }

    public class ListOption<T>
    {
        public T Value { get; set; }
        public string Display { get; set; }
    }

    public class LookupEndPoint
    {
        public string ResourceUri { get; set; }
        public string HttpMethod { get; set; }
        public SearchCriteriaModel Criteria { get; set; }
        public string ServiceName { get; set; }
    }

}
