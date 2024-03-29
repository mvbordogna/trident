using System;
using System.Collections.Generic;
using System.Linq;

using Trident.Extensions.OpenApi.Abstractions;
using Trident.Extensions.OpenApi.Extensions;

using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;

namespace Trident.Extensions.OpenApi.Visitors
{
    /// <summary>
    /// This represents the type visitor for <see cref="List{T}"/>.
    /// </summary>
    public class ListObjectTypeVisitor : TypeVisitor
    {
        /// <inheritdoc />
        public ListObjectTypeVisitor(VisitorCollection visitorCollection)
            : base(visitorCollection)
        {
        }

        /// <inheritdoc />
        public override bool IsVisitable(Type type)
        {
            var isVisitable = this.IsVisitable(type, TypeCode.Object) && type.IsOpenApiArray();

            return isVisitable;
        }

        /// <inheritdoc />
        public override void Visit(IAcceptor acceptor, VisitorState state, KeyValuePair<string, Type> type, NamingStrategy namingStrategy, params Attribute[] attributes)
        {
            var name = this.Visit(acceptor, name: type.Key, title: null, dataType: "array", dataFormat: null, attributes: attributes);

            if (name.IsNullOrWhiteSpace())
            {
                return;
            }

            var instance = acceptor as OpenApiSchemaAcceptor;
            if (instance.IsNullOrDefault())
            {
                return;
            }

            // Gets the schema for the underlying type.
            var underlyingType = type.Value.GetUnderlyingType();
            var types = new Dictionary<string, Type>()
            {
                { underlyingType.GetOpenApiTypeName(namingStrategy), underlyingType }
            };
            var schemas = new Dictionary<string, OpenApiSchema>();

            var subAcceptor = new OpenApiSchemaAcceptor()
            {
                Types = types,
                RootSchemas = instance.RootSchemas,
                Schemas = schemas,
            };

            subAcceptor.Accept(this.VisitorCollection, state, namingStrategy);

            var items = subAcceptor.Schemas.First().Value;

            // Adds the reference to the schema for the underlying type.
            if (this.IsReferential(underlyingType))
            {
                var reference = new OpenApiReference()
                {
                    Type = ReferenceType.Schema,
                    Id = underlyingType.GetOpenApiReferenceId(isDictionary: false, isList: false, namingStrategy)
                };

                items.Reference = reference;
            }

            instance.Schemas[name].Items = items;

            // Adds schemas to the root.
            var schemasToBeAdded = subAcceptor.Schemas
                                              .Where(p => !instance.Schemas.Keys.Contains(p.Key))
                                              .Where(p => p.Value.Type == "object" &&
                                                          p.Value.Format.IsNullOrWhiteSpace() &&
                                                          p.Value.Items.IsNullOrDefault() &&
                                                          p.Value.AdditionalProperties.IsNullOrDefault())
                                              .ToDictionary(p => p.Key, p => p.Value);

            if (!schemasToBeAdded.Any())
            {
                return;
            }

            foreach (var schema in schemasToBeAdded)
            {
                if (instance.RootSchemas.ContainsKey(schema.Key))
                {
                    continue;
                }

                instance.RootSchemas.Add(schema.Key, schema.Value);
            }
        }

        /// <inheritdoc />
        public override bool IsParameterVisitable(Type type)
        {
            var isVisitable = this.IsVisitable(type);

            return isVisitable;
        }

        /// <inheritdoc />
        public override OpenApiSchema ParameterVisit(Type type, NamingStrategy namingStrategy)
        {
            var schema = this.ParameterVisit(dataType: "array", dataFormat: null);

            var underlyingType = type.GetUnderlyingType();
            var items = this.VisitorCollection.ParameterVisit(underlyingType, namingStrategy);

            schema.Items = items;

            return schema;
        }

        /// <inheritdoc />
        public override bool IsPayloadVisitable(Type type)
        {
            var isVisitable = this.IsVisitable(type);

            return isVisitable;
        }

        /// <inheritdoc />
        public override OpenApiSchema PayloadVisit(Type type, NamingStrategy namingStrategy)
        {
            var schema = this.PayloadVisit(dataType: "array", dataFormat: null);

            // Gets the schema for the underlying type.
            var underlyingType = type.GetUnderlyingType();
            var items = this.VisitorCollection.PayloadVisit(underlyingType, namingStrategy);

            // Adds the reference to the schema for the underlying type.
            if (underlyingType.IsReferentialType())
            {
                var reference = new OpenApiReference()
                {
                    Type = ReferenceType.Schema,
                    Id = underlyingType.GetOpenApiReferenceId(isDictionary: false, isList: false, namingStrategy)
                };

                items.Reference = reference;
            }

            schema.Items = items;

            return schema;
        }
    }
}
