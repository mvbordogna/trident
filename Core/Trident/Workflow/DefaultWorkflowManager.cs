using System.Collections.Generic;
using Trident.Business;
using Trident.Domain;
using Trident.Logging;

namespace Trident.Workflow
{
    public class DefaultWorkflowManager<T> : WorkflowManagerBase<T>, IWorkflowManager<T>
        where T : Entity
    {
        public DefaultWorkflowManager(IEnumerable<IWorkflowTask<BusinessContext<T>>> tasks, ILog logger) : base(tasks, logger)
        {
        }
    }
}
