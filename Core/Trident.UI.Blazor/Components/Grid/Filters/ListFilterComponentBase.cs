using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident.Api.Search;
using Trident.Contracts.Api;
using Trident.Extensions;
using Trident.UI.Blazor.Contracts.Services;

namespace Trident.UI.Blazor.Components.Grid.Filters
{
    public abstract class ListFilterComponentBase<TOptions, TEventArgs> : FilterComponentBase<TOptions, TEventArgs>
        where TOptions : ListFilterOptions
        where TEventArgs : FilterEventArgs
    {
        [Inject]
        protected IHttpGenericService HttpService { get; set; }

        [Parameter]
        public string Name { get; set; } = Guid.NewGuid().ToString();

        protected List<ListOption<string>> ListData { get; set; }
        protected bool DisableInitLoad { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (FilterOptions?.ListItems?.Any() ?? false)
            {
                ListData = FilterOptions.ListItems;
            }
            else if (!DisableInitLoad)
            {
                ListData ??= await GetOptions();
            }
        }

        protected async Task<List<ListOption<string>>> GetOptions(string filter = null)
        {
            if (!string.IsNullOrWhiteSpace(FilterOptions?.EndPoint?.ServiceName)
                && !string.IsNullOrWhiteSpace(FilterOptions?.EndPoint?.HttpMethod)
                && !string.IsNullOrWhiteSpace(FilterOptions?.EndPoint?.ResourceUri))
            {
                var criteria = FilterOptions.EndPoint.Criteria.Clone();
                await AppplyFilter(criteria, filter);

                var results = await HttpService.SendRequest<SearchResultsModel<LookupModel, SearchCriteriaModel>>(
                    FilterOptions.EndPoint.ServiceName,
                    FilterOptions.EndPoint.HttpMethod,
                    FilterOptions.EndPoint.ResourceUri,
                    criteria
                    );
                if (results.IsSuccessStatusCode)
                {
                    return results.Model.Results
                         .Select(x => new ListOption<string>() { Display = x.Display, Value = x.Id.ToString() })
                         .ToList();
                }
            }

            return null;
        }

        protected virtual Task AppplyFilter(SearchCriteriaModel criteria, string filter)
        {
            criteria.Keywords = filter;
            return Task.CompletedTask;
        }


    }
}
