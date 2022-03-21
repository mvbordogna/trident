using System;
using System.Collections.Generic;
using System.Linq;

using Trident.Extensions.OpenApi.Abstractions;
using Trident.Extensions.OpenApi.Attributes;
using Trident.Extensions.OpenApi.Extensions;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;

namespace Trident.Extensions.OpenApi.Visitors
{
    /// <summary>
    /// This represents the type visitor for <see cref="object"/>.
    /// </summary>
    public class NullableObjectTypeVisitor : TypeVisitor
    {
        /// <inheritdoc />
        public NullableObjectTypeVisitor(VisitorCollection visitorCollection)
            : base(visitorCollection)
        {
        }

        /// <inheritdoc />
        public override bool IsVisitable(Type type)
        {
            var isVisitable = this.IsVisitable(type, TypeCode.Object) && type.IsOpenApiNullable();

            return isVisitable;
        }

        /// <inheritdoc />
        public override void Visit(IAcceptor acceptor, VisitorState state, KeyValuePair<string, Type> type, NamingStrategy namingStrategy, params Attribute[] attributes)
        {
            var instance = acceptor as OpenApiSchemaAcceptor;
            if (instance.IsNullOrDefault())
            {
                return;
            }

            // Gets the schema for the underlying type.
            var underlyingType = type.Value.GetUnderlyingType();
            var types = new Dictionary<string, Type>()
            {
                { type.Key, underlyingType }
            };
            var schemas = new Dictionary<string, OpenApiSchema>();

            var subAcceptor = new OpenApiSchemaAcceptor()
            {
                Types = types,
                Schemas = schemas,
            };

            subAcceptor.Accept(this.VisitorCollection, state, namingStrategy);

            // Adds the schema for the underlying type.
            var name = subAcceptor.Schemas.First().Key;
            var schema = subAcceptor.Schemas.First().Value;
            schema.Nullable = true;

            // Adds the visibility property.
            if (attributes.Any())
            {
                var visibilityAttribute = attributes.OfType<OpenApiSchemaVisibilityAttribute>().SingleOrDefault();
                if (!visibilityAttribute.IsNullOrDefault())
                {
                    var extension = new OpenApiString(visibilityAttribute.Visibility.ToDisplayName());

                    schema.Extensions.Add("x-ms-visibility", extension);
                }
            }

            instance.Schemas.Add(name, schema);
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
            var underlyingType = type.GetUnderlyingType();
            var schema = this.VisitorCollection.ParameterVisit(underlyingType, namingStrategy);

            schema.Nullable = true;

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
            var underlyingType = type.GetUnderlyingType();
            var schema = this.VisitorCollection.PayloadVisit(underlyingType, namingStrategy);

            schema.Nullable = true;

            return schema;
        }
    }
}
