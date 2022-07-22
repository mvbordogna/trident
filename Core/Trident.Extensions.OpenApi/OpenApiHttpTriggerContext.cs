using System;
using System.Reflection;

using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;
using Trident.Extensions.OpenApi.Abstractions;
using Trident.Extensions.OpenApi.Configurations;
using Trident.Extensions.OpenApi.Enums;
using Trident.Extensions.OpenApi.Extensions;
using Trident.Extensions.OpenApi.Resolvers;
using Trident.Extensions.OpenApi.Visitors;

namespace Trident.Extensions.OpenApi
{
    /// <summary>
    /// Open Api Http Trigger Context
    /// </summary>
    /// <seealso cref="Trident.Extensions.OpenApi.Abstractions.IOpenApiHttpTriggerContext" />
    public class OpenApiHttpTriggerContext : IOpenApiHttpTriggerContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiHttpTriggerContext"/> class.
        /// </summary>
        public OpenApiHttpTriggerContext()
        {
            var host = HostJsonResolver.Resolve();

            this.OpenApiInfo = OpenApiInfoResolver.Resolve(host);
            this.HttpSettings = host.GetHttpSettings();

            var filter = new RouteConstraintFilter();
            var acceptor = new OpenApiSchemaAcceptor();
            var helper = new DocumentHelper(filter, acceptor);

            this.Document = new Document(helper);
            this.SwaggerUI = new SwaggerUI();
        }

        /// <inheritdoc />
        public virtual OpenApiInfo OpenApiInfo { get; }

        /// <inheritdoc />
        public virtual HttpSettings HttpSettings { get; }

        /// <inheritdoc />
        public virtual IDocument Document { get; }

        /// <inheritdoc />
        public virtual ISwaggerUI SwaggerUI { get; }

        /// <inheritdoc />
        public virtual NamingStrategy NamingStrategy { get; } = new CamelCaseNamingStrategy();

        /// <inheritdoc />
        public virtual Assembly GetExecutingAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        /// <inheritdoc />
        public virtual VisitorCollection GetVisitorCollection()
        {
            var collection = VisitorCollection.CreateInstance();

            return collection;
        }

        /// <inheritdoc />
        public virtual OpenApiSpecVersion GetOpenApiSpecVersion(string version = "v2")
        {
            var parsed = Enum.TryParse(version, true, out OpenApiVersionType output)
                             ? output
                             : throw new InvalidOperationException("Invalid Open API version");

            return this.GetOpenApiSpecVersion(parsed);
        }

        /// <inheritdoc />
        public virtual OpenApiSpecVersion GetOpenApiSpecVersion(OpenApiVersionType version = OpenApiVersionType.V2)
        {
            return version.ToOpenApiSpecVersion();
        }

        /// <inheritdoc />
        public virtual OpenApiFormat GetOpenApiFormat(string format = "json")
        {
            if (format.Equals("yml", StringComparison.InvariantCultureIgnoreCase))
            {
                format = "yaml";
            }

            var parsed = Enum.TryParse(format, true, out OpenApiFormatType output)
                             ? output
                             : throw new InvalidOperationException("Invalid Open API format");

            return this.GetOpenApiFormat(parsed);
        }

        /// <inheritdoc />
        public virtual OpenApiFormat GetOpenApiFormat(OpenApiFormatType format = OpenApiFormatType.Json)
        {
            return format.ToOpenApiFormat();
        }

        /// <inheritdoc />
        public virtual string GetSwaggerAuthKey(string key = "OpenApi__ApiKey")
        {
            var value = Environment.GetEnvironmentVariable(key);

            return value;
        }
    }
}
