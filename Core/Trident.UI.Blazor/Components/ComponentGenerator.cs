using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Trident.Contracts.Api.Client;

namespace Trident.UI.Blazor.Components
{
    internal class Parameter
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
    }
    internal interface IComponentGenerator : IServiceProxy
    {
        RenderFragment CreateDynamicComponent(Type componentType, Dictionary<string, object> parameters);
    }

    public class ComponentGenerator  : IComponentGenerator
    {
        public RenderFragment CreateDynamicComponent(Type componentType, Dictionary<string, object> parameters) => builder =>
        {
            builder.OpenComponent(0, componentType);
            int i = 0;
            foreach (var kv in parameters)
            {
                builder.AddAttribute(i, kv.Key, kv.Value);
            }

            builder.CloseComponent();
        };

        private RenderFragment CreateDynamicComponent(Type componentType, List<Parameter> parameters) => builder =>
        {
            builder.OpenComponent(0, componentType);

            foreach (var p in parameters.OrderBy(x => x.Order))
            {
                builder.AddAttribute(p.Order, p.Name, p.Value);
            }

            builder.CloseComponent();
        };
    }
}
