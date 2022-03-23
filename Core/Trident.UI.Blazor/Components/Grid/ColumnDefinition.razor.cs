using Microsoft.AspNetCore.Components;
using System;

namespace Trident.UI.Blazor.Components.Grid
{
    public partial class ColumnDefinition<TItem> : BaseRazorComponent
    {
        [CascadingParameter(Name = "ParentGrid")]
        public PagableGridView<TItem> ParentGrid { get; set; }

        [Parameter]
        public string Property { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public Type PropertyType { get; set; }

        [Parameter]
        public string PropertyMemberPath { get; set; }

        [Parameter]
        public RenderFragment<TItem> Template { get; set; }

        [Parameter]
        public RenderFragment<TItem> EditTemplate { get; set; }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public bool HideColumn { get; set; } = false;

        [Parameter]
        public string FilterType { get; set; }

        [Parameter]
        public object FilterOptions { get; set; }

        [Parameter]
        public bool FilterEnabled { get; set; }

        [Parameter]
        public bool EnableSorting { get; set; } = true;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ParentGrid?.AddColumn(this);
        }
    }

}
