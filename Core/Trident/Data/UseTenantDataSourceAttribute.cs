using Trident.Contracts.Enums;
using System;

namespace Trident.Data
{
    /// <summary>
    /// Class UseTenantDataSourceAttribute. is used as a marking to 
    /// define an entity map to a specific tenant segmented data source in a heterogenous data source system. This class cannot be inherited.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class UseTenantDataSourceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseTenantDataSourceAttribute"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        public UseTenantDataSourceAttribute(string dataSource)
        {
            this.DataSource = dataSource;
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public string DataSource { get; set; }
    }
}