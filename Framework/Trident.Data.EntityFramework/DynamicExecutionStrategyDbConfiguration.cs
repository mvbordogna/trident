using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Runtime.Remoting.Messaging;

namespace Trident.Data.EntityFramework
{
    /// <summary>
    /// Class DynamicExecutionStrategyDbConfiguration.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbConfiguration" />
    public class DynamicExecutionStrategyDbConfiguration:DbConfiguration   
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicExecutionStrategyDbConfiguration" /> class.
        /// </summary>
        public DynamicExecutionStrategyDbConfiguration()
        {
            this.SetExecutionStrategy("System.Data.SqlClient", () => SuspendExecutionStrategy
              ? (IDbExecutionStrategy)new DefaultExecutionStrategy()
              : new SqlAzureExecutionStrategy());
            SetDefaultConnectionFactory(new LocalDbConnectionFactory("v11.0"));
        }



        /// <summary>
        /// Gets or sets a value indicating whether [suspend execution strategy].
        /// </summary>
        /// <value><c>true</c> if [suspend execution strategy]; otherwise, <c>false</c>.</value>
        public static bool SuspendExecutionStrategy
        {
            get
            {
                return (bool?)CallContext.LogicalGetData("SuspendExecutionStrategy") ?? false;
            }
            set
            {
                CallContext.LogicalSetData("SuspendExecutionStrategy", value);
            }
        }
    }
}
