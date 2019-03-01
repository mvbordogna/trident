using System;

namespace Trident.Data
{
    /// <summary>
    /// Class UseSharedDataSourceAttribute is used as a marking to 
    /// define an entity map to a specific data source in a heterogenous data source system. This class cannot be inherited.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <remarks>When this marker is not present on an entity, the data source will default 
    /// to what has been configured in the abstract conext factory as the default data source</remarks>
    /// <seealso cref="System.Attribute" />
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class UseSharedDataSourceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseSharedDataSourceAttribute"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        public UseSharedDataSourceAttribute(string dataSource=null)
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