using System.Collections.Generic;
using Trident.Business;
using Trident.Domain;

namespace Trident.Workflow
{
    public class DefaultWorkflowManager<T> : WorkflowManagerBase<T>, IWorkflowManager<T>
        where T : Entity
    {
        public DefaultWorkflowManager(IEnumerable<IWorkflowTask<BusinessContext<T>>> tasks) : base(tasks)
        {
        }
    }
}
