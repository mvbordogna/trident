using AutoMapper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Trident.Validation;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Trident.Contracts.Routes;
using Trident.Logging;
using Org=Trident.Samples.AzureFunctions.Models.Organization;
using Trident.Azure.Functions;
using Trident.Samples.Domain.Entities;
using Trident.Contracts;
using Trident.Mapper;
using System.Collections.Generic;


namespace Trident.Samples.AzureFunctions.Functions.Organization
{
    public sealed class OrganizationFunctions : HttpFunctionApiBase<Org.Organization, OrganizationEntity, Guid>
    {
        private readonly ILog _appLogger;
        private readonly IMapperRegistry _mapper;


        public OrganizationFunctions(ILog appLogger, IMapperRegistry mapper, IManager<Guid, OrganizationEntity> organizationManager) : base(appLogger, mapper, organizationManager)
        {
            _appLogger = appLogger;
            _mapper = mapper;

        }
        [Function("GetOrganizationById")]
        [OpenApiOperation(operationId: "GetOrganizationById", tags: new[] { nameof(OrganizationFunctions) }, Summary = nameof(OrganizationRoutes.GetById))]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Org.Organization))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError)]
        public async Task<HttpResponseData> GetOrganizationById([HttpTrigger(AuthorizationLevel.Anonymous, OrganizationRoutes.GetByIdMethod, Route = OrganizationRoutes.GetById)] HttpRequestData req, Guid id)
        {
            return await GetById(req, id, "GetOrganizationById");
        }

        [Function("GetOrganizations")]
        [OpenApiOperation(operationId: "GetOrganizations", tags: new[] { nameof(OrganizationFunctions) }, Summary = nameof(OrganizationRoutes.GetAll))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Org.Organization>))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError)]
        public async Task<HttpResponseData> GetOrganizations([HttpTrigger(AuthorizationLevel.Anonymous, OrganizationRoutes.GetAllMethod, Route = OrganizationRoutes.GetAll)] HttpRequestData req)
        {
            return await GetAll(req, "GetOrganizations");
        }

        [Function("CreateOrganization")]
        [OpenApiOperation(operationId: "CreateOrganization", tags: new[] { nameof(OrganizationFunctions) }, Summary = nameof(OrganizationRoutes.Create))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Org.Organization))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Org.Organization))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError)]
        public async Task<HttpResponseData> CreateOrganization([HttpTrigger(AuthorizationLevel.Anonymous, OrganizationRoutes.CreateMethod, Route = OrganizationRoutes.Create)] HttpRequestData req)
        {
            HttpResponseData response;

            try
            {
                response = await Create(req, "CreateReimbursement");
            }
            catch (ValidationRollupException validationRollupException)
            {
                _appLogger.Error<OrganizationFunctions>(messageTemplate: $"CreateOrganization - Validation Exception - {validationRollupException.Message}");
                response = req.CreateResponse();
                await response.WriteAsJsonAsync(validationRollupException.ValidationResults);
                response.StatusCode = HttpStatusCode.BadRequest;
            }

            return response;
        }
    }
}
