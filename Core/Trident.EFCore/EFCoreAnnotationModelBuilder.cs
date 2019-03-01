using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Trident.Data;
using Trident.EFCore.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trident.Contracts.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Trident.EFCore
{
    /// <summary>
    /// Provides an abstract implemenation of an assembly scanning IDbModelBuilder
    /// Implements the <see cref="Trident.EFCore.Contracts.IEFCoreModelBuilder" />
    /// </summary>
    /// <typeparam name="TEntity">The base Type that is the primary filter for entities to be registered with the EFDataContext</typeparam>
    /// <seealso cref="Trident.EFCore.Contracts.IEFCoreModelBuilder" />
    public abstract class EFCoreAnnotationDirectiveModelBuilder<TEntity> : IEFCoreModelBuilder
    {
        /// <summary>
        /// The pad lock
        /// </summary>
        private static object _padLock = new object();
        /// <summary>
        /// The compiled model
        /// </summary>
        private IModel _compiledModel;
        /// <summary>
        /// The model assemblies
        /// </summary>
        private Assembly[] _modelAssemblies = null;
        /// <summary>
        /// The convention set
        /// </summary>
        private ConventionSet _conventionSet = null;
        /// <summary>
        /// The data source type
        /// </summary>
        private DataSourceType _dataSourceType;
        /// <summary>
        /// The data source
        /// </summary>
        private string _dataSource;

        /// <summary>
        /// Construct a new instance of the AnnotationDirectiveModelBuilder class assuming it will scan its Host assembly for Models to register with EF
        /// </summary>
        protected EFCoreAnnotationDirectiveModelBuilder()
            : this(DataSourceType.Default, "", false) { }

        /// <summary>
        /// Construct a new instance of the AnnotationDirectiveModelBuilder class that will scan its Host assembly for Models to register with EF if specified
        /// and any of the model Assemblies specified. Assumes that the Entities to be register inherit from Trident.Domain.Entity
        /// </summary>
        /// <param name="dataSourceType">Type of the data source.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="scanSelfAssembly">Scans the Assmily in which the AnnotationDirectiveModelBuilder lives for models to register with EF</param>
        /// <param name="modelAssemblies">The model assemblies.</param>
        protected EFCoreAnnotationDirectiveModelBuilder(DataSourceType dataSourceType, string dataSource, bool scanSelfAssembly, params Assembly[] modelAssemblies)
        {
            this._dataSourceType = dataSourceType;
            this._dataSource = dataSource;

            List<Assembly> assemblies = new List<Assembly>();

            if (modelAssemblies == null || scanSelfAssembly)
            {
                assemblies.Add(this.GetType().Assembly);
            }

            if (modelAssemblies != null)
            {
                assemblies.AddRange(modelAssemblies);
            }

            this._modelAssemblies = assemblies.ToArray();
        }

        /// <summary>
        /// Builds the model mappings.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        public void AppendModelMappings(ModelBuilder modelBuilder)
        {
            var types = this._modelAssemblies.SelectMany(x => x.GetTypes());
            var mapTypes = from t in types
                           where
                               t.BaseType != null
                               && t.BaseType != typeof(object)
                               && typeof(TEntity).IsAssignableFrom(t)
                               && !t.IsAbstract
                               && ApplyDataSourceFilter(t)
                           select t;

            foreach (var mapType in mapTypes)
            {
                var modelBinding = modelBuilder.Entity(mapType);
                this.ApplyAttributeSpecs(mapType, modelBinding);
            }
        }

        protected virtual void ApplyAttributeSpecs(Type entityType,  EntityTypeBuilder modelBinding)
        {
           
        }

        /// <summary>
        /// Applies the data source filter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ApplyDataSourceFilter(Type type)
        {
            var dataSourceName = string.Empty;
            if (_dataSourceType == DataSourceType.Shared)
            {
                dataSourceName = type.GetCustomAttribute<UseSharedDataSourceAttribute>()?.DataSource.ToString()
                    ?? SharedDataSource.DefaultDB.ToString();
            }
            else if (_dataSourceType == DataSourceType.Tenant)
            {
                dataSourceName = type.GetCustomAttribute<UseTenantDataSourceAttribute>()?.DataSource.ToString();
            }

            return dataSourceName == this._dataSource;
        }
    }
}
