using System;
using System.Collections.Generic;

using Trident.Extensions.OpenApi.Abstractions;

using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;

namespace Trident.Extensions.OpenApi.Visitors
{
    /// <summary>
    /// This represents the type visitor for <see cref="double"/>.
    /// </summary>
    public class DoubleTypeVisitor : TypeVisitor
    {
        /// <inheritdoc />
        public DoubleTypeVisitor(VisitorCollection visitorCollection)
            : base(visitorCollection)
        {
        }

        /// <inheritdoc />
        public override bool IsVisitable(Type type)
        {
            var isVisitable = this.IsVisitable(type, TypeCode.Double);

            return isVisitable;
        }

        /// <inheritdoc />
        public override void Visit(IAcceptor acceptor, VisitorState state, KeyValuePair<string, Type> type, NamingStrategy namingStrategy, params Attribute[] attributes)
        {
            this.Visit(acceptor, name: type.Key, title: null, dataType: "number", dataFormat: "double", attributes: attributes);
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
            return this.ParameterVisit(dataType: "number", dataFormat: "double");
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
            return this.PayloadVisit(dataType: "number", dataFormat: "double");
        }
    }
}
