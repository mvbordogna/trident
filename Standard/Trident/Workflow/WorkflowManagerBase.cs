using Trident.Domain;
using Trident.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident.Business;

namespace Trident.Workflow
{

    /// <summary>
    /// Class WorkFlowManagerBase.
    /// Implements the <see cref="Trident.Workflow.IWorkflowManager" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Workflow.IWorkflowManager" />
    public abstract class WorkflowManagerBase<TEntity> : IWorkflowManager
        where TEntity : Entity
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowManagerBase{TEntity}" /> class.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        protected WorkflowManagerBase(IEnumerable<IWorkflowTask> tasks)
        {
            if (tasks != null)
                this.Tasks = tasks.OrderBy(x => x.RunOrder).ToList();
        }

        /// <summary>
        /// Validates the specified object to validate.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stage">The stage.</param>
        /// <returns>List&lt;WorkFlowResult&gt;.</returns>
        public async Task Run(BusinessContext context, OperationStage stage = OperationStage.All)
        {
            context.GuardIsNotNull(nameof(context));
            context.Target.GuardIsNotNull(nameof(context.Target));
            await RunTasks(context, stage);
        }

        /// <summary>
        /// Runs all registered tasks.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stage">The stage.</param>
        /// <returns>Task.</returns>
        /// <exception cref="Trident.Workflow.WorkFlowCancelledException">Task {task.GetType()} cancelled the workflow during {stage.ToString()} stage of the {context.Operation.ToString()}</exception>
        protected virtual async Task RunTasks(BusinessContext context, OperationStage stage)
        {

            if (this.Tasks != null)
            {
                GuardOnlyOneStageBitSet(stage);

                var tasksToRun = (stage == OperationStage.All)
                    ? this.Tasks
                    : this.Tasks.Where(x => x.Stage.HasFlag(stage)).ToList();

                foreach (var task in tasksToRun)
                {
                    if (await task.ShouldRun(context) && !await task.Run(context))
                        throw new WorkFlowCancelledException($"Task {task.GetType()} cancelled the workflow during {stage.ToString()} stage of the {context.Operation.ToString()} operation", task.GetType());
                }
            }
        }

        /// <summary>
        /// Guards the only one stage bit set.
        /// </summary>
        /// <param name="stage">The stage.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        private void GuardOnlyOneStageBitSet(OperationStage stage)
        {
            const string err = "The Run operation of the Workflow manager only supports a single Flag when requesting a run operation. Tasks can be configured to support multiple operations";

            int intVal = ((int)stage);
            bool singleBitIsSet = (intVal != 0 && (intVal & (intVal - 1)) == 0) || intVal == 0;
            if (!singleBitIsSet)
                throw new System.InvalidOperationException(err);
        }

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <value>The tasks.</value>
        protected List<IWorkflowTask> Tasks { get; private set; }
    }

}





