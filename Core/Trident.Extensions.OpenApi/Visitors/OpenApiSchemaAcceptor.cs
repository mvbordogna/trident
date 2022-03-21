using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Trident.Extensions.OpenApi.Attributes;
using Trident.Extensions.OpenApi.Extensions;

using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;

namespace Trident.Extensions.OpenApi.Visitors
{
    /// <summary>
    /// This represents the acceptor entity for <see cref="OpenApiSchema"/>.
    /// </summary>
    public class OpenApiSchemaAcceptor : IOpenApiSchemaAcceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiSchemaAcceptor"/> class.
        /// </summary>
        public OpenApiSchemaAcceptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiSchemaAcceptor"/> class.
        /// </summary>
        /// <param name="rootSchemas">List of <see cref="OpenApiSchema"/> instances as key/value pair representing the root schemas.</param>
        /// <param name="schemas">List of <see cref="OpenApiSchema"/> instances as key/value pair.</param>
        /// <param name="types">List of <see cref="Type"/> objects.</param>
        public OpenApiSchemaAcceptor(Dictionary<string, OpenApiSchema> rootSchemas, Dictionary<string, OpenApiSchema> schemas, Dictionary<string, Type> types)
        {
            this.RootSchemas = rootSchemas.ThrowIfNullOrDefault();
            this.Schemas = schemas.ThrowIfNullOrDefault();
            this.Types = types.ThrowIfNullOrDefault();
        }

        /// <inheritdoc />
        public Dictionary<string, OpenApiSchema> RootSchemas { get; set; } = new Dictionary<string, OpenApiSchema>();

        /// <inheritdoc />
        public Dictionary<string, OpenApiSchema> Schemas { get; set; } = new Dictionary<string, OpenApiSchema>();

        /// <inheritdoc />
        public Dictionary<string, Type> Types { get; set; } = new Dictionary<string, Type>();

        /// <inheritdoc />
        public Dictionary<string, PropertyInfo> Properties { get; set; } = new Dictionary<string, PropertyInfo>();

     
        /// <inheritdoc />
        public void Accept(VisitorCollection collection, VisitorState state, NamingStrategy namingStrategy)
        {
            // Checks the properties only.
            if (this.Properties.Any())
            {             
                foreach (var property in this.Properties)
                {
                    var visibilityAttribute = property.Value.GetCustomAttribute<OpenApiSchemaVisibilityAttribute>(inherit: false);
                    var hash = $"{property.Value.DeclaringType.FullName}|{property.Value.PropertyType}|{property.Value.Name}";
                    if (!state.HasVisited(hash))
                    {
                        foreach (var visitor in collection.Visitors)
                        {
                            state.Visited(hash);

                            if (!visitor.IsVisitable(property.Value.PropertyType))
                            {
                                continue;
                            }                            
                            var type = new KeyValuePair<string, Type>(property.Key, property.Value.PropertyType);
                            visitor.Visit(this, state, type, namingStrategy, visibilityAttribute);
                        }
                    }
                }

                return;
            }

            // Checks the types only.
            foreach (var type in this.Types)
            {
                foreach (var visitor in collection.Visitors)
                {
                    if (!visitor.IsVisitable(type.Value))
                    {
                        continue;
                    }

                    visitor.Visit(this, state, type, namingStrategy);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VisitorState
    {
        private readonly HashSet<string> visited = new HashSet<string>();

        /// <summary>
        /// Determines whether the specified hast test has visited.
        /// </summary>
        /// <param name="hastTest">The hast test.</param>
        /// <returns>
        ///   <c>true</c> if the specified hast test has visited; otherwise, <c>false</c>.
        /// </returns>
        public bool HasVisited(string hastTest)
        {
            return visited.Contains(hastTest);
        }

        /// <summary>
        /// Visiteds the specified hash.
        /// </summary>
        /// <param name="hash">The hash.</param>
        public void Visited(string hash)
        {
            visited.Add(hash);
        }
    }
}
