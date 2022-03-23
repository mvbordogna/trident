using System;

namespace Trident.UI.Blazor.Components.Grid.Filters
{
    internal class DatetimeFilterArgs : FilterEventArgs<DateFilter>
    {
        public DatetimeFilterArgs(DateFilter source) : base(source) { }
        public Bounds Bound { get; set; }
    }
}
