namespace Trident.Extensions.OpenApi.Configurations
{
    /// <summary>
    /// This represents the settings entity for HTTP in "host.json".
    /// </summary>
    public class HttpSettings
    {
        /// <summary>
        /// Gets or sets the route prefix. Default value is "api".
        /// </summary>
        public virtual string RoutePrefix { get; set; } = "api";
    }
}
