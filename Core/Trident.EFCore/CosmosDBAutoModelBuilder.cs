using Trident.Data;
using Trident.Domain;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Microsoft.EntityFrameworkCore;

namespace Trident.EFCore
{
    /// <summary>
    /// Class CosmosDBAutoModelBuilder.
    /// Implements the <see cref="Trident.EFCore.EFCoreAnnotationDirectiveModelBuilder{Trident.Domain.Entity}" />
    /// </summary>
    /// <seealso cref="Trident.EFCore.EFCoreAnnotationDirectiveModelBuilder{Trident.Domain.Entity}" />
    public class CosmosDBAutoModelBuilder : EFCoreAnnotationDirectiveModelBuilder<Entity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDBAutoModelBuilder" /> class.
        /// </summary>
        /// <param name="dataSourceType">Type of the data source.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="assembliesToScan">Converts to scan.</param>
        public CosmosDBAutoModelBuilder(
            DataSourceType dataSourceType,
            string dataSource,         
            params Assembly[] assembliesToScan)
            : base(dataSourceType, dataSource, false, assembliesToScan)
        {
        }



        protected override void ApplyAttributeSpecs(Type entityType, EntityTypeBuilder modelBinding)
        {
            var nameValue = entityType.GetCustomAttribute<ContainerAttribute>()?.Name;
            nameValue = string.IsNullOrWhiteSpace(nameValue)
                ? entityType.Name
                : nameValue;

            modelBinding.ToContainer(nameValue);

        }

    }
}
