using System.Threading.Tasks;

namespace Trident.Workflow
{
    /// <summary>
    /// Class WorkflowTaskBase.
    /// Implements the <see cref="Trident.Workflow.IWorkflowTask{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Trident.Workflow.IWorkflowTask{T}" />
    public abstract class WorkflowTaskBase<T> : IWorkflowTask<T>
        where T : WorkflowContext
    {
        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <value>The ordinal.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public abstract int RunOrder { get; }

        /// <summary>
        /// Gets the stage.
        /// </summary>
        /// <value>The stage.</value>
        public abstract OperationStage Stage { get; }

        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> Run(object context)
        {
            return await this.Run((T)context);
        }

        /// <summary>
        /// Shoulds the run.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> ShouldRun(object context)
        {
            return await ShouldRun((T)context);
        }

        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>WorkflowError.</returns>
        public abstract Task<bool> Run(T context);

        /// <summary>
        /// Shoulds the run.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public abstract Task<bool> ShouldRun(T context);


    }
}
