namespace Trident.UI.Blazor.Components.Grid.Filters
{
    public class FilterEventArgs
    {
        protected FilterEventArgs() { }

        public FilterEventArgs(IFilterCompoent source)
        {
            FilterComponent = source;
        }

        public IFilterCompoent FilterComponent { get; protected set; }
    }

    public class FilterEventArgs<TSource> : FilterEventArgs
        where TSource : class, IFilterCompoent
    {
        public FilterEventArgs(TSource source)
        {
            FilterComponent = source;
        }

        public new TSource FilterComponent
        {
            get
            {
                return base.FilterComponent as TSource;
            }
            protected set
            {
                base.FilterComponent = value;
            }
        }

        public object OriginalValue { get; internal set; }
        public object NewValue { get; internal set; }
    }
}
