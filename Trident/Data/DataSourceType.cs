namespace Trident.Data
{
    /// <summary>
    /// Enum DataSourceType: Designates the scope of a data source
    /// </summary>  
    public enum DataSourceType
    {
        /// <summary>
        /// The default
        /// </summary>
        Default = 0,
        /// <summary>
        /// Designates a data source as being shared data source, meaning its the 
        /// same source across multiple customers in a SaaS solution
        /// </summary>
        Shared = 1,
        /// <summary>
        /// Designates a data source as being only for one or a group of tenants, 
        /// that must be resolved based on customer specific identification scheme
        /// </summary>
        Tenant = 2
    }
}
