using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trident.UI.Client.Contracts.Models;

namespace Trident.UI.Blazor.Components.Forms
{
    public partial class DualListBox<TItem, TValue> : ComponentBase
        where TItem : ModelBase<TValue>
    {
        private List<TItem> RemainingData { get; set; } = new List<TItem>();
        private List<TItem> SelectedData { get; set; } = new List<TItem>();
        private RadzenListBox<EnumerableQuery<TValue>> RightControl { get; set; }
        private RadzenListBox<EnumerableQuery<TValue>> LeftControl { get; set; }
        private Expression<Func<TItem, object>> _sortExpression;
        private Func<TItem, object> _sortExpressionCompiled;

        [Parameter]
        public IEnumerable<TItem> SourceData { get; set; } = new List<TItem>();

        [Parameter]
        public IEnumerable<TValue> SelectedValues { get; set; } = new List<TValue>();

        [Parameter]
        public string ValueProperty { get; set; }

        [Parameter]
        public string TextProperty { get; set; }

        [Parameter]
        public EventCallback<IEnumerable<TItem>> OnSelectChange { get; set; } = new EventCallback<IEnumerable<TItem>>();

        [Parameter]
        public Expression<Func<TItem, object>> SortExpression
        {
            get => _sortExpression;
            set
            {
                _sortExpression = value;
                if (value != null)
                {
                    _sortExpressionCompiled = _sortExpression.Compile();
                }
            }
        }



        protected override void OnParametersSet()
        {
            if (SourceData.Any())
            {
                SelectedData = SourceData
                    .Where(x => SelectedValues.Contains(x.Id))
                    .OrderBy(_sortExpressionCompiled)
                    .ToList();

                RemainingData = SourceData
                    .Where(x => !SelectedValues.Contains(x.Id))
                    .OrderBy(_sortExpressionCompiled)
                    .ToList();
            }
        }

        private async Task MoveRight()
        {
            SelectedData = MoveSelection(LeftControl, RemainingData, SelectedData);
            await RaiseOnSelectChangeHandler();
        }

        private async Task MoveLeft()
        {
            RemainingData = MoveSelection(RightControl, SelectedData, RemainingData);
            await RaiseOnSelectChangeHandler();
        }

        private async Task RaiseOnSelectChangeHandler()
        {
            if (OnSelectChange.HasDelegate)
            {
                await OnSelectChange.InvokeAsync(SelectedData);
            }
        }

        private List<TItem> MoveSelection(RadzenListBox<EnumerableQuery<TValue>> sourceControl, List<TItem> source, List<TItem> destination)
        {
            var selectedItemIds = sourceControl.Value as IEnumerable<TValue>;
            if (selectedItemIds != null)
            {
                var movedItems = source.Where(x => selectedItemIds.Contains(x.Id)).ToList();
                source.RemoveAll(x => selectedItemIds.Contains(x.Id));
                destination.AddRange(movedItems);
                return destination.OrderBy(_sortExpressionCompiled).ToList();
            }
            return destination;
        }

    }
}
