using System;
using Trident.Business;
using Trident.Data.Contracts;
using Trident.TestTargetProject.Domain;
using Trident.Validation;
using Trident.Workflow;

namespace Trident.TestTargetProject
{
    public class OrganisationManager : ManagerBase<Guid, Organisation>
    {
        public OrganisationManager(IProvider<Guid, Organisation> provider, 
            IValidationManager<Organisation> validationManager, 
            IWorkflowManager<Organisation> workflowManager 
            ) 
            : base(provider, validationManager, workflowManager)
        {
        }
    }
}
