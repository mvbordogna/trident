using System;
using Trident.Business;
using Trident.Data.Contracts;
using Trident.Samples.Domain.Entities;
using Trident.TestTargetProject.Business.Test.ValidationRules;
using Trident.Validation;
using Trident.Workflow;

namespace Trident.TestTargetProject
{
    public class OrganisationManager : ManagerBase<Guid, OrganizationEntity>
    {
        public OrganisationManager(IProvider<Guid, OrganizationEntity> provider,
            IValidationManager<OrganizationEntity> validationManager, 
            IWorkflowManager<OrganizationEntity> workflowManager 
            ) 
            : base(provider, validationManager, workflowManager)
        {
        }
    }
}
