using System;
using Trident.Business;
using Trident.Data.Contracts;
using Trident.Logging;
using Trident.Samples.Domain.Entities;
using Trident.Validation;
using Trident.Workflow;

namespace Trident.TestTargetProject
{
    public class OrganisationManager : ManagerBase<Guid, OrganizationEntity>
    {
        public OrganisationManager(ILog logger, IProvider<Guid, OrganizationEntity> provider,
            IValidationManager<OrganizationEntity> validationManager,
            IWorkflowManager<OrganizationEntity> workflowManager
            )
            : base(logger, provider, validationManager, workflowManager)
        {
        }
    }
}
