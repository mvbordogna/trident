using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Trident.Data;
using Trident.EFCore.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Trident.Contracts.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Trident.Domain;
using Trident.EFCore.Json;
using Newtonsoft.Json.Linq;

namespace Trident.EFCore
{
    /// <summary>
    /// Provides an abstract implemenation of an assembly scanning IDbModelBuilder
    /// Implements the <see cref="IEFCoreModelBuilder" />
    /// </summary>
    /// <typeparam name="TEntity">The base Type that is the primary filter for entities to be registered with the EFDataContext</typeparam>
    /// <seealso cref="IEFCoreModelBuilder" />
    public abstract class EFCoreAnnotationDirectiveModelBuilder<TEntity> : IEFCoreModelBuilder
    {
        /// <summary>
        /// The pad lock
        /// </summary>
        private static object _padLock = new object();
        /// <summary>
        /// The model assemblies
        /// </summary>
        private Assembly[] _modelAssemblies = null;

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
        /// and any of the model Assemblies specified. Assumes that the Entities to be register inherit from  Trident.Domain.Entity
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
        public void AppendModelMappings(ModelBuilder modelBuilder, IEntityMapFactory mapFactory)
        {
            var types = this._modelAssemblies.SelectMany(x => x.GetTypes());
            var mapTypes = (from t in types
                            where
                                t.BaseType != null
                                && t.BaseType != typeof(object)
                                && typeof(TEntity).IsAssignableFrom(t)
                                && !t.IsAbstract
                                && ApplyDataSourceFilter(t)
                            select t).ToList();

            AddDynamicClass(mapTypes);

            foreach (var mapType in mapTypes)
            {
                if (!mapType.IsGenericTypeDefinition || mapType.IsConstructedGenericType)
                {
                    var binderMethod = modelBuilder.GetType()
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .First(x => x.Name == nameof(modelBuilder.Entity) && x.IsGenericMethod)
                        .MakeGenericMethod(mapType);

                    var genericEntityTypeBuilder = binderMethod.Invoke(modelBuilder, new object[] { }) as EntityTypeBuilder;
                    ApplySequenceFilter(mapType, genericEntityTypeBuilder, modelBuilder);
                    ApplyAttributeSpecs(mapType, genericEntityTypeBuilder);

                    var maps = mapFactory.GetMapsFor(mapType);

                    foreach (var map in maps)
                    {
                        map.Configure(genericEntityTypeBuilder);
                    }
                }
            }

            AddJsonFields(modelBuilder);         
        }

        protected virtual void AddDynamicClass(List<Type> mapTypes )
        {
         
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

        private void ApplySequenceFilter(Type entityType, EntityTypeBuilder modelBinding, ModelBuilder modelBuilder)
        {
            var seqAttr = entityType.GetCustomAttribute<SequenceAttribute>();
            if (seqAttr != null)
            {
                var all = entityType.GetProperties().Where(x => x.Name == nameof(Entity.Id));
                var info = all.FirstOrDefault(x => x.DeclaringType == typeof(EntityBase<>)) ?? all.First();
                modelBuilder.HasSequence(info.PropertyType, seqAttr.Name, seqAttr.Schema);
                if (!seqAttr.ManualSequence)
                {
                    if (seqAttr.DataAccessProvider == DataAccessProviderTypes.SqlServer)
                    {
                        modelBinding.Property(nameof(Entity.Id))
                            .UseHiLo(seqAttr.Name, schema: seqAttr.Schema);
                    }
                    else
                    {
                        modelBinding.Property(nameof(Entity.Id))
                            .HasDefaultValueSql($"NEXT value for {seqAttr.Schema}.{seqAttr.Name}");
                    }
                }
            }
        }

        public static void AddJsonFields(ModelBuilder modelBuilder, bool skipConventionalEntities = true)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {

                var typeBase = typeof(TypeBase);
                if (skipConventionalEntities)
                {
                    var typeConfigurationSource = typeBase
                        .GetField("_configurationSource", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.GetValue(entityType)?.ToString();
                    if (Enum.TryParse(typeConfigurationSource, out ConfigurationSource typeSource) &&
                        typeSource == ConfigurationSource.Convention) continue;
                }

                var ignoredMembers =
                    typeBase.GetField("_ignoredMembers", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.GetValue(entityType) as Dictionary<string, ConfigurationSource>;

                bool HasJsonAttribute(PropertyInfo propertyInfo) =>
                    propertyInfo != null &&
                    propertyInfo.CustomAttributes.Any(a => a.AttributeType == typeof(JsonFieldAttribute));

                bool HasPolymorphicJasonAttribute(PropertyInfo propertyInfo) =>
                    propertyInfo != null &&
                    propertyInfo.CustomAttributes.Any(a => a.AttributeType == typeof(PolymorphicJsonFieldAttribute));

                bool NotIgnored(PropertyInfo property) =>
                    property != null &&
                    !ignoredMembers.ContainsKey(property.Name) &&
                    property.CustomAttributes.All(a => 
                        a.AttributeType != typeof(NotMappedAttribute) 
                        && a.AttributeType != typeof(OwnedHierarchyAttribute)
                    );

                foreach (var clrProperty in entityType.ClrType.GetProperties()
                    .Where(x => NotIgnored(x) && (HasJsonAttribute(x) || HasPolymorphicJasonAttribute(x))))
                {
                    var property = modelBuilder.Entity(entityType.ClrType)
                        .Property(clrProperty.PropertyType, clrProperty.Name);
                    var modelType = clrProperty.PropertyType;

                    var converterOpenGenericType = (HasPolymorphicJasonAttribute(clrProperty))
                         ? typeof(PolymorphicJsonValueConverter<>)
                         : typeof(JsonValueConverter<>);
                 
                    var converterType = converterOpenGenericType.MakeGenericType(modelType);
                    var converter = (ValueConverter) Activator.CreateInstance(converterType, new object[] {null});
                    property.Metadata.SetValueConverter(converter);

                    var valueComparer = typeof(JsonValueComparer<>).MakeGenericType(modelType);
                    property.Metadata.SetValueComparer(
                        (ValueComparer) Activator.CreateInstance(valueComparer, new object[0]));
                }
            }

        }       

    }


    internal class DynamicValueConverter<T> : ValueConverter<JObject, object>
    {
        public DynamicValueConverter(ConverterMappingHints hints = default) : base(
            v => Serialize<T>(v),
            v => Deserialize<T>(v), hints)
        {

           
        }

        private static object Serialize<W>(object v)
        {
            return v;
        }

        private static JObject Deserialize<W>(object v)
        {
            return (JObject)v;
        }


    }

    public static class EFDynamicTypeRepository
    {     
        public static Dictionary<string, Type> StaticDynamicTypes = new Dictionary<string, Type>();
    }
}
