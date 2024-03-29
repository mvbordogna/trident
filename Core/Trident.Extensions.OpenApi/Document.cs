using System.IO;

using System.Reflection;
using System.Threading.Tasks;

using Trident.Extensions.OpenApi.Abstractions;
using Trident.Extensions.OpenApi.Extensions;
using Trident.Extensions.OpenApi.Visitors;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;

namespace Trident.Extensions.OpenApi
{
    /// <summary>
    /// This represents the document entity handling Open API document.
    /// </summary>
    public class Document : IDocument
    {
        private readonly IDocumentHelper _helper;

        private NamingStrategy _strategy;
        private VisitorCollection _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        public Document(IDocumentHelper helper)
        {
            this._helper = helper.ThrowIfNullOrDefault();
        }

        /// <inheritdoc />
        public OpenApiDocument OpenApiDocument { get; private set; }

        /// <inheritdoc />
        public IDocument InitialiseDocument()
        {
            this.OpenApiDocument = new OpenApiDocument()
            {
                Components = new OpenApiComponents()
            };

            return this;
        }

        /// <inheritdoc />
        public IDocument AddMetadata(OpenApiInfo info)
        {
            this.OpenApiDocument.Info = info;

            return this;
        }

        /// <inheritdoc />
        public IDocument AddServer(HttpRequestData req, string routePrefix)
        {
            var prefix = string.IsNullOrWhiteSpace(routePrefix) ? string.Empty : $"/{routePrefix}";
            var baseUrl = $"{req.Url.Scheme}://{req.Url.Host}:{req.Url.Port}{prefix}";

            this.OpenApiDocument.Servers.Add(new OpenApiServer { Url = baseUrl });

            return this;
        }

        /// <inheritdoc />
        public IDocument AddNamingStrategy(NamingStrategy strategy)
        {
            this._strategy = strategy.ThrowIfNullOrDefault();

            return this;
        }

        /// <inheritdoc />
        public IDocument AddVisitors(VisitorCollection collection)
        {
            this._collection = collection.ThrowIfNullOrDefault();

            return this;
        }

        /// <inheritdoc />
        public IDocument Build(string assemblyPath)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);

            return this.Build(assembly);
        }

        /// <inheritdoc />
        public IDocument Build(Assembly assembly)
        {
            if (this._strategy.IsNullOrDefault())
            {
                this._strategy = new DefaultNamingStrategy();
            }

            var paths = new OpenApiPaths();

            var methods = this._helper.GetHttpTriggerMethods(assembly);
            foreach (var method in methods)
            {
                var trigger = this._helper.GetHttpTriggerAttribute(method);
                if (trigger.IsNullOrDefault())
                {
                    continue;
                }

                var function = this._helper.GetFunctionNameAttribute(method);
                if (function.IsNullOrDefault())
                {
                    continue;
                }

                var path = this._helper.GetHttpEndpoint(function, trigger);
                if (path.IsNullOrWhiteSpace())
                {
                    continue;
                }

                var verb = this._helper.GetHttpVerb(trigger);

                var item = this._helper.GetOpenApiPath(path, paths);
                var operations = item.Operations;

                var operation = this._helper.GetOpenApiOperation(method, function, verb);
                if (operation.IsNullOrDefault())
                {
                    continue;
                }

                operation.Parameters = this._helper.GetOpenApiParameters(method, trigger, this._strategy, this._collection);
                operation.RequestBody = this._helper.GetOpenApiRequestBody(method, this._strategy, this._collection);
                operation.Responses = this._helper.GetOpenApiResponses(method, this._strategy, this._collection);

                operations[verb] = operation;
                item.Operations = operations;

                paths[path] = item;
            }

            this.OpenApiDocument.Paths = paths;
            this.OpenApiDocument.Components.Schemas = this._helper.GetOpenApiSchemas(methods, this._strategy, this._collection);
            this.OpenApiDocument.Components.SecuritySchemes = this._helper.GetOpenApiSecuritySchemes();

            return this;
        }

        /// <inheritdoc />
        public async Task<string> RenderAsync(OpenApiSpecVersion version, OpenApiFormat format)
        {
            var result = await Task.Factory
                                   .StartNew(() => this.Render(version, format))
                                   .ConfigureAwait(false);

            return result;
        }

        private string Render(OpenApiSpecVersion version, OpenApiFormat format)
        {
            using (var sw = new StringWriter())
            {
                this.OpenApiDocument.Serialise(sw, version, format);

                return sw.ToString();
            }
        }
    }
}
