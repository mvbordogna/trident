using System.Threading.Tasks;
using Trident.Business;
using Trident.Domain;

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
        Task Run(BusinessContext context, OperationStage operationStage = OperationStage.All);
        void RunSync(BusinessContext context, OperationStage operationStage = OperationStage.All);
    }


    public interface IWorkflowManager<T> : IWorkflowManager
        where T : Entity
    {

    }
}
