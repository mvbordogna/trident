using System.Collections.Generic;
using System.Threading.Tasks;
using Trident.Business;

namespace Trident.Workflow
{

    /// <summary>
    /// Interface IWorkflowRule
    /// </summary>
    public interface IWorkflowTask
    {
        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <value>The ordinal.</value>
        int RunOrder { get; }

        /// <summary>
        /// Gets the stage.
        /// </summary>
        /// <value>The stage.</value>
        OperationStage Stage { get; }

        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> Run(object context);

        /// <summary>
        /// Shoulds the run.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ShouldRun(object context);
    }

    /// <summary>
    /// Interface IWorkflowRule
    /// Implements the <see cref="TridentOptionsBuilder.Workflow.IWorkflowTask" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TridentOptionsBuilder.Workflow.IWorkflowTask" />
    public interface IWorkflowTask<in T> : IWorkflowTask
        where T : BusinessContext
    {
        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>WorkflowError.</returns>
        Task<bool> Run(T context);

        /// <summary>
        /// Shoulds the run.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ShouldRun(T context);
    }
}
