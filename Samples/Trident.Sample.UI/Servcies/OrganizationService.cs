using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Trident.Contracts.Api.Client;
using Trident.Contracts.Routes;
using Trident.Samples.Contracts.Models;
using Trident.Samples.Contracts.Services;
using Trident.UI.Client;

namespace Trident.Sample.UI.Servcies
{
    [Service(Services.TridentSampleApi, Services.Resources.organization)]
    public class OrganizationService : ServiceBase<OrganizationService, OrganizationModel, Guid>, IOrganizationService
    {
        private readonly ILogger<OrganizationService> _logger;
        public OrganizationService(ILogger<OrganizationService> logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory)
        {
            _logger = logger;
        }
        
        public async Task<List<OrganizationModel>> GetOrganizations()
        {
            Logger.LogInformation("Get Organizations - Calling API");

            try
            {
                var organizationList = new List<OrganizationModel>();

                var response = await SendRequest<List<OrganizationModel>>(
                                                                            OrganizationRoutes.GetAllMethod,
                                                                            OrganizationRoutes.GetAll);

                if (response.IsSuccessStatusCode)
                {
                    organizationList = response.Model;
                }

                return organizationList;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Get organization List - API Error getting accounts");
                throw;
            }
        }

        public async Task<OrganizationModel> GetOrganizationById(string Id)
        {
            var response = await SendRequest<OrganizationModel>(OrganizationRoutes.GetByIdMethod, OrganizationRoutes.GetById.Replace("{id}", Id));
            return response.Model;
        }

        public async Task<Response<OrganizationModel>> CreateOrganization(OrganizationModel organization)
        {
            return await SendRequest<OrganizationModel>(OrganizationRoutes.CreateMethod, OrganizationRoutes.CreateMethod, organization);
        }
    }
}
