using System;
using System.IO;

using Microsoft.OpenApi;
using Microsoft.OpenApi.Writers;

namespace Trident.Extensions.OpenApi
{
    /// <summary>
    /// This represents the factory entity to create a write instance based on the Open API document format.
    /// </summary>
    public static class OpenApiWriterFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IOpenApiWriter"/> based on the Open API document format.
        /// </summary>
        /// <param name="format"><see cref="OpenApiFormat"/> value.</param>
        /// <param name="writer"><see cref="TextWriter"/> instance.</param>
        /// <returns><see cref="IOpenApiWriter"/> instance.</returns>
        public static IOpenApiWriter CreateInstance(OpenApiFormat format, TextWriter writer)
        {
            switch (format)
            {
                case OpenApiFormat.Json:
                    return new OpenApiJsonWriter(writer);

                case OpenApiFormat.Yaml:
                    return new OpenApiYamlWriter(writer);

                default:
                    throw new InvalidOperationException("Invalid Open API format");
            }
        }
    }
}
