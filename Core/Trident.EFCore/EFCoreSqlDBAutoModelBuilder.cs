using Trident.Data;
using Trident.Domain;
using System.Reflection;

namespace Trident.EFCore
{
    /// <summary>
    /// Class EFCoreSqlDBAutoModelBuilder.
    /// Implements the <see cref="TridentOptionsBuilder.EFCore.EFCoreAnnotationDirectiveModelBuilder{TridentOptionsBuilder.Domain.Entity}" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.EFCore.EFCoreAnnotationDirectiveModelBuilder{TridentOptionsBuilder.Domain.Entity}" />
    public class EFCoreSqlDBAutoModelBuilder:EFCoreAnnotationDirectiveModelBuilder<Entity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreSqlDBAutoModelBuilder"/> class.
        /// </summary>
        /// <param name="dataSourceType">Type of the data source.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="assembliesToScan">Converts to scan.</param>
        public EFCoreSqlDBAutoModelBuilder(DataSourceType dataSourceType, string dataSource, params Assembly[] assembliesToScan)
             : base(dataSourceType, dataSource, false,  modelAssemblies: assembliesToScan)
        { }
    }
}
