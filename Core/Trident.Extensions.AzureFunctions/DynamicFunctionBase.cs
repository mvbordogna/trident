using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Scholar.Framework.Core.Configuration;
using Scholar.Framework.Core.Domain;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Scholar.Framework.Azure.Common
{
    public class DynamicHttpFunctionApiBase : IFunctionController
    {
        protected IDynamicHttpApiFactory ApiFactory { get; }

        public DynamicHttpFunctionApiBase(IDynamicHttpApiFactory apiFactory)
        {
            ApiFactory = apiFactory;           
        }


        // [ClaimsAuthorize(Claims.Role, Claims.Values.Write)]

        //[OpenApiOperation(operationId: "Search")]
        //[OpenApiRequestBody(contentType: "appliation/json", bodyType: typeof(SearchCriteriaModel))]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(SearchResultsModel<DynamicApiModel<T>, SearchCriteriaModel>))]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest)]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError)]
        [Function("DynamicEntitySearch")]
        public async Task<HttpResponseData> Search(
           [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "{version}/{resource}/search")] HttpRequestData req, string vesion, string resource)
        {
            IDynamicHttpApi api = ApiFactory.Get(vesion, resource);
            return await api.Search(req, $"Search {resource}");
        }

        [Function("DynamicEntityGetById")]
        //[OpenApiOperation(operationId: "GetRoleById")]
        //[OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Role))]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound)]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError)]
        public async Task<HttpResponseData> GetById(
          [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "{version}/{resource}/{id}")] HttpRequestData req, string version, string resource,
          Guid id)
        {
            IDynamicHttpApi api = ApiFactory.Get(version, resource);            
            return await api.GetById(req, id, $"Get{resource}ById");
        }

        [Function("DynamicEntitySearchUpdate")]
        //[OpenApiOperation(operationId: "UpdateRole")]
        //[OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
        //[OpenApiRequestBody(contentType: "appliation/json", bodyType: typeof(Role))]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Role))]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest)]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError)]
        public async Task<HttpResponseData> Update(
         [HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "{version}/{resource}/{id}")] HttpRequestData req, string version, string resource, Guid id)
        {
            IDynamicHttpApi api = ApiFactory.Get(version, resource);
            return await api.Update(req, id, $"Update{resource}");
        }

        [Function("DynamicEntitySearchCreate")]
        //[OpenApiOperation(operationId: "CreateRole")]
        //[OpenApiRequestBody(contentType: "appliation/json", bodyType: typeof(Role))]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Role))]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound)]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError)]
        public async Task<HttpResponseData> Create(
         [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "{version}/{resource}")] HttpRequestData req, string version, string resource)
        {
            IDynamicHttpApi api = ApiFactory.Get(version, resource);
            return await api.Create(req, $"Create{resource}");
        }

        [Function("DynamicEntitySearchDelete")]
        //[OpenApiOperation(operationId: "DeleteRole")]
        //[OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
        //[OpenApiRequestBody(contentType: "appliation/json", bodyType: typeof(Role))]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Role))]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest)]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError)]
        public async Task<HttpResponseData> Delete(
         [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "{version}/{resource}/{id}")] HttpRequestData req, string version, string resource, Guid id)
        {
            IDynamicHttpApi api = ApiFactory.Get(version, resource);
            return await api.Delete(req, id, $"Delete{resource}");
        }
    }


}
