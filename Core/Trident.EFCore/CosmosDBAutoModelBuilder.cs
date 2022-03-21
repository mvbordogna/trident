using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Trident.Data;
using Trident.Domain;

namespace Trident.EFCore
{
    /// <summary>
    /// Class CosmosDBAutoModelBuilder.
    /// Implements the <see cref="Entity" />
    /// </summary>
    /// <seealso cref="Entity" />
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
            var discriminatorAttr = entityType.GetCustomAttribute<DiscriminatorAttribute>();
            var containerAttr = entityType.GetCustomAttribute<ContainerAttribute>();


            var nameValue = containerAttr?.Name;
            nameValue = string.IsNullOrWhiteSpace(nameValue)
                ? entityType.Name
                : nameValue;

            modelBinding.ToContainer(nameValue);

            if (!string.IsNullOrWhiteSpace(containerAttr?.PartitionKey))
            {
                modelBinding.HasPartitionKey(containerAttr.PartitionKey);
            }

            var properties = entityType.GetProperties(BindingFlags.Public |
               BindingFlags.Instance | BindingFlags.DeclaredOnly
               ).ToList();

            if (discriminatorAttr != null)
            {
                var dFlags = BindingFlags.Public | BindingFlags.Instance;
                var discriminatorType = entityType.GetProperty(discriminatorAttr.Property, dFlags);

                if (discriminatorType != null)
                {
                    modelBinding.HasDiscriminator(discriminatorType.Name, discriminatorType.PropertyType)
                        .HasValue(discriminatorAttr.Value);
                }
            }

            ConfigureDefaultTypeConverters(modelBinding, properties);
            ConfigureOwnedComplexTypes(modelBinding, properties);
            ConfigureOwnedListsTypes(modelBinding, properties);
        }

        private void ConfigureDefaultTypeConverters(EntityTypeBuilder modelBinding, IEnumerable<PropertyInfo> properties)
        {
            properties.Select(x => new { Property = x, OwnedAttribute = x.GetCustomAttribute<OwnedHierarchyAttribute>() })
                .Where(x => CosmosDefaultConverters.HasConverter(x.Property.PropertyType) && x.OwnedAttribute == null)
                .ToList()
                .ForEach((x) =>
                {
                    if (CosmosDefaultConverters.Converters.TryGetValue(x.Property.PropertyType, out var converter))
                    {
                        modelBinding.Property(x.Property.Name).HasConversion(converter);
                    }
                });
        }

        private void ConfigureOwnedComplexTypes(EntityTypeBuilder modelBinding, IEnumerable<PropertyInfo> properties)
        {
            //owned complex types
            properties.Select(x => new { Property = x, OwnedAttribute = x.GetCustomAttribute<OwnedHierarchyAttribute>() })
                .Where(x => x.OwnedAttribute != null && IsOwnableComplexProperty(x.Property))
                .ToList()
                .ForEach((x) =>
                {
                    var rootName = x.OwnedAttribute.FieldName ?? x.Property.Name;

                    if (IsOwnableComplexProperty(x.Property))
                    {
                        modelBinding.OwnsOne(x.Property.PropertyType, rootName, (b) =>
                        {
                            b.WithOwner();
                            MarkAsOwnedRecursively(b, x.Property.PropertyType.GetProperties(), new VisitorState());
                        });
                    }
                });
        }

        private void ConfigureOwnedListsTypes(EntityTypeBuilder modelBinding, IEnumerable<PropertyInfo> properties)
        {
            //owned lists           
            properties.Select(x => new { Property = x, OwnedAttribute = x.GetCustomAttribute<OwnedHierarchyAttribute>() })
                .Where(x => x.OwnedAttribute != null && IsOwnableListProperty(x.Property) && !(x.Property.PropertyType is IEnumerable<object>))
                .ToList()
                .ForEach((x) =>
                {
                    var rootName = x.OwnedAttribute.FieldName ?? x.Property.Name;
                    var listItemType = x.Property.PropertyType.GenericTypeArguments.FirstOrDefault();
                    if (listItemType != null)
                    {
                        modelBinding.OwnsMany(listItemType, rootName, (b) =>
                        {
                            b.WithOwner();
                            MarkAsOwnedRecursively(b, listItemType.GetProperties(), new VisitorState());
                        });
                    }
                });
        }


        private Func<PropertyInfo, bool> IsOwnableListProperty = (x) =>
            typeof(IEnumerable).IsAssignableFrom(x.PropertyType)
            && !x.PropertyType.IsPrimitive()
            && x.PropertyType.IsGenericType
            && !x.PropertyType.GetGenericArguments().Any(y => y.IsPrimitive());

        private Func<PropertyInfo, bool> IsOwnableComplexProperty = (x) =>
            !typeof(IEnumerable).IsAssignableFrom(x.PropertyType)
            && !x.PropertyType.IsPrimitive()
            && typeof(object) != x.PropertyType;

        private Func<PropertyInfo, bool> IsKeyProperty = (x) =>
            x.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() != null;

        private Func<PropertyInfo, bool> HasJsonConverterDirective = (x) =>
           x.GetCustomAttribute<JsonFieldAttribute>() != null || x.GetCustomAttribute<PolymorphicJsonFieldAttribute>() != null;

        private Func<PropertyInfo, bool> IsMarkedNotMapped = (x) => x.GetCustomAttribute<NotMappedAttribute>() != null;


        private void MarkAsOwnedRecursively(OwnedNavigationBuilder builder, PropertyInfo[] properties, VisitorState state)
        {
            foreach (var prop in properties)
            {
                if (!IsMarkedNotMapped(prop) && !HasJsonConverterDirective(prop) && !state.HasVisited(prop))
                {
                    // Properties with defined type converters (primarly consists of primitives and Type)
                    if (!IsKeyProperty(prop)
                        && CosmosDefaultConverters.HasConverter(prop.PropertyType) && prop.SetMethod != null)
                    {
                        if (CosmosDefaultConverters.Converters.TryGetValue(prop.PropertyType, out var converter))
                        {
                            state.Visited(prop);
                            builder.Property(prop.Name).HasConversion(converter);
                        }
                        continue;
                    }

                    if (IsOwnableComplexProperty(prop))
                    {
                        state.Visited(prop);
                        builder.OwnsOne(prop.PropertyType, prop.Name, (b) =>
                        {
                            b.WithOwner();
                            MarkAsOwnedRecursively(b, prop.PropertyType.GetProperties(), state);
                        });

                        continue;
                    }

                    if (IsOwnableListProperty(prop))
                    {
                        state.Visited(prop);
                        var listItemType = prop.PropertyType.GenericTypeArguments.FirstOrDefault();
                        if (listItemType != null)
                        {
                            if (listItemType == typeof(Type))
                            {
                                builder.Property(prop.Name).HasConversion(new Converters.TypeValueConverter());
                            }
                            else
                            {
                                builder.OwnsMany(listItemType, prop.Name, (b) =>
                                {
                                    b.WithOwner();
                                    MarkAsOwnedRecursively(b, listItemType.GetProperties(), state);
                                });
                            }
                        }
                        continue;
                    }
                }
            }
        }

        private class VisitorState
        {
            private readonly HashSet<PropertyInfo> visited = new HashSet<PropertyInfo>();
            public bool HasVisited(PropertyInfo hash)
            {
                return visited.Contains(hash);
            }

            public void Visited(PropertyInfo hash)
            {
                visited.Add(hash);
            }
        }
    }
}
