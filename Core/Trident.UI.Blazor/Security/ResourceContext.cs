using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trident.UI.Blazor.Security
{
    public class ResourceContext
    {
        public ResourceContext(Type pageType, IDictionary<string, object> routeValues)
        {
            PageType = pageType;
            RouteValues = routeValues;
        }
        public Type PageType { get; }
        public IDictionary<string, object> RouteValues { get; }

        public IReadOnlyList<AuthorizeClaimAttribute> GetClaimRequirements()
        {
            return PageType.GetCustomAttributes<AuthorizeClaimAttribute>(true).ToList().AsReadOnly();
        }
    }
}
