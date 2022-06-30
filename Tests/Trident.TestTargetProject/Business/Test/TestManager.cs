using System;
using Trident.Business;
using Trident.Data.Contracts;
using Trident.TestTargetProject.Business.Test.ValidationRules;
using Trident.TestTargetProject.Domain;
using Trident.Validation;
using Trident.Workflow;
using Trident.Logging;
namespace Trident.TestTargetProject
{
    public class OrganisationManager : ManagerBase<Guid, Organisation>
    {
        public OrganisationManager(
            ILog logger,
            IProvider<Guid, Organisation> provider,
            IValidationManager<Organisation> validationManager, 
            IWorkflowManager<Organisation> workflowManager 
            ) 
            : base(logger, provider, validationManager, workflowManager)
        {
        }
    }
}
