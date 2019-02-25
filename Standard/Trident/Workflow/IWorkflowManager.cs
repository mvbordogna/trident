using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trident.Workflow
{
    /// <summary>
    /// Class IWorkflowManager.
    /// </summary>
    public interface IWorkflowManager
    {
        /// <summary>
        /// Validates the specified object to validate.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="operationStage">The operation stage.</param>
        /// <returns>List&lt;WorkflowResult&gt;.</returns>
        Task Run(WorkflowContext context, OperationStage operationStage = OperationStage.All);

    }
}
