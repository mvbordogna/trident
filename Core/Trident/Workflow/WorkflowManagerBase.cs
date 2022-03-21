using Trident.Domain;
using Trident.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident.Business;
using Trident.Logging;

namespace Trident.Workflow
{

    /// <summary>
    /// Class WorkFlowManagerBase.
    /// Implements the <see cref="TridentOptionsBuilder.Workflow.IWorkflowManager" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Workflow.IWorkflowManager" />
    public abstract class WorkflowManagerBase<TEntity> : IWorkflowManager
        where TEntity : Entity
    {
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowManagerBase{TEntity}" /> class.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        protected WorkflowManagerBase(IEnumerable<IWorkflowTask> tasks, ILog logger)
        {
            if (tasks != null)
                this.Tasks = tasks.OrderBy(x => x.RunOrder).ToList();
            logger.GuardIsNotNull(nameof(logger));
            _logger = logger;
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

        public void RunSync(BusinessContext context, OperationStage stage = OperationStage.All)
        {
            context.GuardIsNotNull(nameof(context));
            context.Target.GuardIsNotNull(nameof(context.Target));
            RunTasksSync(context, stage);
        }

        /// <summary>
        /// Runs all registered tasks.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stage">The stage.</param>
        /// <returns>Task.</returns>
        /// <exception cref="TridentOptionsBuilder.Workflow.WorkFlowCancelledException">Task {task.GetType()} cancelled the workflow during {stage.ToString()} stage of the {context.Operation.ToString()}</exception>
        protected virtual async Task RunTasks(BusinessContext context, OperationStage stage)
        {

            if (this.Tasks != null)
            {
                _logger.Debug(messageTemplate: $"Starting workflow tasks for stage: {stage} on manager: {this.GetType().GetFriendlyName()}");
                GuardOnlyOneStageBitSet(stage);

                var tasksToRun = (stage == OperationStage.All)
                    ? this.Tasks
                    : this.Tasks.Where(x => x.Stage.HasFlag(stage)).ToList();
                _logger.Information(messageTemplate: $"running {tasksToRun.Count} workflow tasks: {string.Join(",", tasksToRun.Select(t => t.GetType().GetFriendlyName()))} to run for stage: {stage} on manager: {this.GetType().GetFriendlyName()}");
                foreach (var task in tasksToRun)
                {
                    var friendlyName = task.GetType().GetFriendlyName();
                    if (await task.ShouldRun(context))
                    {
                        var failed = !await task.Run(context);
                        if (failed) throw new WorkFlowCancelledException($"Task {friendlyName} cancelled the workflow during {stage} stage of the {context.Operation} operation", task.GetType());
                        _logger.Debug(messageTemplate: $"The workflow task: {friendlyName} ran and was successful for stage: {stage} on manager: {friendlyName}");
                    }
                    else
                    {
                        _logger.Debug(messageTemplate: $"The workflow task: {friendlyName} should not run for stage: {stage} on manager: {friendlyName}");
                    }
                }
            }
        }

        /// <summary>
        /// Runs all registered tasks.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stage">The stage.</param>
        /// <returns>Task.</returns>
        /// <exception cref="TridentOptionsBuilder.Workflow.WorkFlowCancelledException">Task {task.GetType()} cancelled the workflow during {stage.ToString()} stage of the {context.Operation.ToString()}</exception>
        protected virtual void RunTasksSync(BusinessContext context, OperationStage stage)
        {

            if (this.Tasks != null)
            {
                GuardOnlyOneStageBitSet(stage);

                var tasksToRun = (stage == OperationStage.All)
                    ? this.Tasks
                    : this.Tasks.Where(x => x.Stage.HasFlag(stage)).ToList();

                foreach (var task in tasksToRun)
                {
                    if (Task.Run(async () => await task.ShouldRun(context)).Result)
                    {
                        if (!Task.Run(async () => await task.Run(context)).Result)
                        {
                            throw new WorkFlowCancelledException($"Task {task.GetType()} cancelled the workflow during {stage.ToString()} stage of the {context.Operation.ToString()} operation", task.GetType());
                        }
                    }
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





