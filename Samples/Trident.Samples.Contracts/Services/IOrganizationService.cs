using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trident.Contracts.Api.Client;
using Trident.Samples.Contracts.Models;

namespace Trident.Samples.Contracts.Services
{
    public interface IOrganizationService : IServiceProxyBase<OrganizationModel, Guid>
    {
        Task<List<OrganizationModel>> GetOrganizations();

        Task<OrganizationModel> GetOrganizationById(string Id);

        Task<Response<OrganizationModel>> CreateOrganization(OrganizationModel organization);
    }
}
