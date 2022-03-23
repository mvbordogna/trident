using System.Threading.Tasks;
using Trident.Business;
using Trident.Samples.Domain.Entities;
using Trident.Workflow;

namespace Trident.TestTargetProject.Business.Test.WorkflowTasks
{
    public class OrgWorkflowTask : Workflow.WorkflowTaskBase<BusinessContext<OrganizationEntity>>
    {
        public override int RunOrder => 1;

        public override OperationStage Stage => OperationStage.All;
    
        public override Task<bool> Run(BusinessContext<OrganizationEntity> context)
        {
            return Task.FromResult(true);
        }
        
        public override Task<bool> ShouldRun(BusinessContext<OrganizationEntity> context)
        {
            return Task.FromResult(true);
        }
    }
}
