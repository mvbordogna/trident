using System.Text.RegularExpressions;

namespace Trident.Extensions.OpenApi.Configurations
{
    /// <summary>
    /// This represents the entity for route constraint filter.
    /// </summary>
    public class RouteConstraintFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteConstraintFilter"/> class.
        /// </summary>
        public RouteConstraintFilter()
        {
            this.Filter = new Regex(this.Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        public virtual string Pattern { get; } = @"^(\{[^\{\:]+)(\:.+)(\})$";

        /// <summary>
        /// Gets the replacement.
        /// </summary>
        public virtual string Replacement { get; } = "$1$3";

        /// <summary>
        /// Gets the <see cref="Regex"/> instance as filter.
        /// </summary>
        public virtual Regex Filter { get; }
    }
}
