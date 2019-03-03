using System.Threading.Tasks;
using Trident.Business;
using Trident.TestTargetProject.Domain;
using Trident.Workflow;

namespace Trident.TestTargetProject.Business.Test.WorkflowTasks
{
    public class OrgWorkflowTask : Workflow.WorkflowTaskBase<BusinessContext<Organisation>>
    {
        public override int RunOrder => 1;

        public override OperationStage Stage => OperationStage.All;
    
        public override Task<bool> Run(BusinessContext<Organisation> context)
        {
            return Task.FromResult(true);
        }
        
        public override Task<bool> ShouldRun(BusinessContext<Organisation> context)
        {
            return Task.FromResult(true);
        }
    }
}
