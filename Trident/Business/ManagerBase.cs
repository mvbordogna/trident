using Trident.Contracts;
using Trident.Data.Contracts;
using Trident.Extensions;
using Trident.Mapper;
using Trident.Transactions;
using Trident.Validation;
using Trident.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trident.Domain;
using Trident.Search;

namespace Trident.Business
{
    /// <summary>
    ///  Provides an abstract base implemenation of a manager class.
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="Trident.Contracts.IManager{TId, TEntity, TSummary, TCriteria}" />
    /// <seealso cref="Trident.Contracts.IManager{TEntity, TSummary, TCriteria}" />
    public abstract class ManagerBase<TId, TEntity, TSummary, TCriteria> : ReadOnlyManagerBase<TEntity, TSummary, TCriteria>,
            IManager<TId, TEntity, TSummary, TCriteria>
        where TEntity : EntityBase<TId>
        where TSummary : Entity
        where TCriteria : SearchCriteria
    {
        private readonly IValidationManager _validationManager;
        private readonly IWorkflowManager _workflowManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerBase{TId,TEntity,TSummary,TCriteria}" /> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="validationManager">The validation manager.</param>
        /// <param name="workflowManager">The workflow manager.</param>
        protected ManagerBase(
            IMapperRegistry mapper,
            IProvider<TId, TEntity, TSummary, TCriteria> provider,
            IValidationManager validationManager,
            IWorkflowManager workflowManager = null) : base(provider)
        {
            mapper.GuardIsNotNull(nameof(mapper));
            provider.GuardIsNotNull(nameof(provider));
            validationManager.GuardIsNotNull(nameof(validationManager));
            Mapper = mapper;
            Provider = provider;
            _validationManager = validationManager;
            _workflowManager = workflowManager;
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>The provider.</value>
        protected new IProvider<TId, TEntity, TSummary, TCriteria> Provider { get; }

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <value>The mapper.</value>
        protected IMapperRegistry Mapper { get; }

        [NonTransactional]
        public async Task<IEnumerable<TEntity>> GetByIds(IEnumerable<TId> ids, bool loadChildren = false)
        {
            return await Provider.GetByIds(ids, loadChildren: loadChildren);
        }

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        public async Task<TEntity> Save(TEntity entity, bool deferCommit = false)
        {
            return await this.Save(entity, deferCommit, null);
        }


        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <param name="contextBag">Contextual values that can be inspected during validation rule or workflow tasks</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>        
        protected async Task<TEntity> Save(TEntity entity, bool deferCommit, IDictionary<string, object> contextBag = null)
        {
            var existing = await GetOriginal(entity);
            var isNew = existing == null;

            var workflowContext = await CreateWorkflowContext(isNew ? Operation.Insert : Operation.Update, entity, existing, contextBag);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, isNew ? OperationStage.BeforeInsert : OperationStage.BeforeUpdate);
            var context = await CreateValidationContext(isNew ? Operation.Insert : Operation.Update, workflowContext.Target, workflowContext.Original, contextBag);
            await _validationManager.Validate(context);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.PostValidation);
            await (isNew
                ? Provider.Insert(entity, deferCommit)
                : Provider.Update(entity, deferCommit));
            await SaveChildren(entity, existing, deferCommit);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, isNew ? OperationStage.AfterInsert : OperationStage.AfterUpdate);
            return entity;
        }


        public async Task<IEnumerable<TEntity>> BulkSave(IEnumerable<TEntity> entities)
        {
            entities.GuardIsNotNull(nameof(entities));
            var existingDict = await GetOriginals(entities);
            int counter = 0;
            int lastEntityIndex = entities.Count() - 1;
            foreach (var entity in entities)
            {
                var existing = existingDict.ContainsKey(entity.Id) ? existingDict[entity.Id] : null;
                var isNew = existing == null;

                var workflowContext = await CreateWorkflowContext(isNew ? Operation.Insert : Operation.Update, entity, existing);
                if (_workflowManager != null) await _workflowManager.Run(workflowContext, isNew ? OperationStage.BeforeInsert : OperationStage.BeforeUpdate);
                var context = await CreateValidationContext(isNew ? Operation.Insert : Operation.Update, workflowContext.Target, workflowContext.Original, workflowContext.ContextBag);
                await _validationManager.Validate(context);
                if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.PostValidation);
                await (isNew
                    ? Provider.Insert(entity, counter != lastEntityIndex)
                    : Provider.Update(entity, counter != lastEntityIndex));
                //TODO: needed to think about this for bulk
                //await this.BulkSaveChildren(entities, existing);
                if (_workflowManager != null) await _workflowManager.Run(workflowContext, isNew ? OperationStage.AfterInsert : OperationStage.AfterUpdate);

                counter++;
            }

            return entities;
        }
       
        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">The defer commit until the commit is called on the manager.</param>
        /// <returns>System.Threading.Tasks.Task&lt;TEntity&gt;.</returns>
        /// <exception cref="InvalidOperationException">Cannot insert an entity that already exists, use PUT Method instead of POST.</exception>
        public async Task<TEntity> Insert(TEntity entity, bool deferCommit = false)
        {
            var existing = await GetOriginal(entity);
            if (existing != null)
            {
                throw new InvalidOperationException("Cannot insert an entity that already exists, use PUT Method instead of POST.");
            }


            var workflowContext = await CreateWorkflowContext(Operation.Insert, entity, null);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.BeforeInsert);
            var context = await CreateValidationContext(Operation.Insert, workflowContext.Target, workflowContext.Original, workflowContext.ContextBag);
            await _validationManager.Validate(context);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.PostValidation);
            await Provider.Insert(entity, deferCommit);
            await SaveChildren(entity, existing, deferCommit);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.AfterInsert);
            return entity;
        }



        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">The defer commit.</param>
        /// <returns>System.Threading.Tasks.Task&lt;TEntity&gt;.</returns>
        public async Task<TEntity> Update(TEntity entity, bool deferCommit = false)
        {
            var existing = await GetOriginal(entity);

            var workflowContext = await CreateWorkflowContext(Operation.Update, entity, existing);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.BeforeUpdate);
            var context = await CreateValidationContext(Operation.Update, workflowContext.Target, workflowContext.Original, workflowContext.ContextBag);
            await _validationManager.Validate(context);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.PostValidation);
            await Provider.Update(entity, deferCommit);
            await SaveChildren(entity, existing, deferCommit);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.AfterUpdate);
            return entity;
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">The defer commit.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> Delete(TEntity entity, bool deferCommit = false)
        {
            var existing = await GetOriginal(entity);
            var workflowContext = await CreateWorkflowContext(Operation.Delete, entity, existing);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.BeforeDelete);
            var context = await CreateValidationContext(Operation.Delete, workflowContext.Target, workflowContext.Original, workflowContext.ContextBag);
            await _validationManager.Validate(context);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.PostValidation);
            await DeleteChildren(existing, deferCommit);
            await Provider.Delete(entity, deferCommit);
            if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.AfterDelete);
            return true;
        }


        public async Task<bool> BulkDelete(IEnumerable<TEntity> entities)
        {
            entities.GuardIsNotNull(nameof(entities));
            var existingDict = await GetOriginals(entities);
            int counter = 0;
            int lasEntityIndex = entities.Count() - 1;
            foreach (var entity in entities)
            {
                var existing = existingDict.ContainsKey(entity.Id) ? existingDict[entity.Id] : null;
                var workflowContext = await CreateWorkflowContext(Operation.Delete, entity, existing);
                if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.BeforeDelete);
                var context = await CreateValidationContext(Operation.Delete, workflowContext.Target, workflowContext.Original, workflowContext.ContextBag);
                await _validationManager.Validate(context);
                if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.PostValidation);
                await DeleteChildren(existing, counter != lasEntityIndex);
                await Provider.Delete(entity, counter != lasEntityIndex);
                if (_workflowManager != null) await _workflowManager.Run(workflowContext, OperationStage.AfterDelete);
                counter++;
            }
            return true;
        }

        /// <summary>
        /// Deletes all the entities matching the ids in the list.
        /// </summary>
        /// <param name="entityIds">The entity ids.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> BulkDelete(IEnumerable<TId> entityIds)
        {  
            return await BulkDelete(await Get(x => entityIds.Contains(x.Id)));
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">The defer commit.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Boolean&gt;.</returns>
        public async Task<TEntity> Patch(TId id, bool deferCommit = false, params Action<TEntity>[] patches)
        {
            var destination = await GetById(id);
            GuardPatch(destination);
            return await Patch(destination, deferCommit, patches: patches);
        }


        public async Task<TEntity> Patch(TId id, Dictionary<string, object> patches, IDictionary<string, Action<TEntity>> overridePatches = null, bool deferCommit = false)
        {
            var destination = await GetById(id);
            GuardPatch(destination);
            var actions = CreatePatchActions(patches, overridePatches);
            return await Patch(destination, deferCommit, patches: actions.ToArray());
        }

        private static void GuardPatch(TEntity destination)
        {
            if (destination == null)
            {
                throw new NotSupportedException("Patch is not supported for inserts.");
            }
        }

        protected async Task<TEntity> Patch(TEntity destination, bool deferCommit = false, IDictionary<string, object> contextBag = null, params Action<TEntity>[] patches)
        {
            foreach (var patch in patches)
            {
                patch(destination);
            }

            return await Save(destination, deferCommit, contextBag);
        }

        protected async Task<TEntity> Patch(TEntity destination, Dictionary<string, object> patches, bool deferCommit = false)
        {
            var actions = CreatePatchActions(patches);
            return await Patch(destination, deferCommit, patches: actions.ToArray());
        }


        private IEnumerable<Action<TEntity>> CreatePatchActions(Dictionary<string, object> filterBy, IDictionary<string, Action<TEntity>> overridePatches = null)
        {
            var list = new List<Action<TEntity>>();
            overridePatches = overridePatches ?? new Dictionary<string, Action<TEntity>>();
            foreach (var key in filterBy.Keys)
            {
                if (!overridePatches.ContainsKey(key))
                {
                    var sourceValue = filterBy[key];
                    var paramExpression = Expression.Parameter(typeof(TEntity), key);
                    var keyPropertyExpression = Expression.Property(paramExpression, key);
                    var parsedValue = TypeExtensions.ParseToTypedObject(sourceValue, keyPropertyExpression.Type);
                    var constantExpression = Expression.Constant(parsedValue);
                    var assignExpression = Expression.Assign(keyPropertyExpression, constantExpression);
                    var assignmentExpression = Expression.Lambda<Action<TEntity>>(assignExpression, paramExpression);
                    list.Add(assignmentExpression.Compile());
                }
                else
                {
                    list.Add(overridePatches[key]);
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the original.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>System.Threading.Tasks.Task&lt;TEntity&gt;.</returns>
        protected virtual async Task<TEntity> GetOriginal(TEntity entity)
        {
            if (entity.Id != null && !Equals(entity.Id, default(TId)))
            {
                return await Provider.GetById(entity.Id, true, true);
            }

            return null;
        }

        protected virtual async Task<Dictionary<TId, TEntity>> GetOriginals(IEnumerable<TEntity> entities)
        {
            if (entities?.Any() ?? false)
            {
                var entityKeys = entities
                    .Where(x => !Equals(x.Id, default(TId)))
                    .Select(x => x.Id).ToList();

                return (await Provider.Get(filter: x => entityKeys.Contains(x.Id), noTracking: true))
                    .ToDictionary(x => x.Id);
            }

            return null;
        }
        
        /// <summary>
        /// Assures the entity identity is updated when blank and entity already exists.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="original">The original.</param>
        protected virtual void AssureEntityIdentity(TEntity entity, TEntity original)
        {
            if (original != null && entity.Id == null && !Equals(entity.Id, default(TId)))
            {
                entity.Id = original.Id;
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="original">The original.</param>
        /// <returns>TValidationContext.</returns>
        protected virtual Task<ValidationContext<TEntity>> CreateValidationContext(Operation operation, TEntity entity, TEntity original, IDictionary<string, Object> contextBag = null)
        {
            return Task.FromResult(new ValidationContext<TEntity>(entity, original, contextBag)
            {
                Operation = operation
            });
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="original">The original.</param>
        /// <returns>TValidationContext.</returns>
        protected virtual Task<WorkflowContext<TEntity>> CreateWorkflowContext(Operation operation, TEntity entity, TEntity original, IDictionary<string, Object> contextBag = null)
        {
            return Task.FromResult(new WorkflowContext<TEntity>(entity, original, contextBag)
            {
                Operation = operation
            });
        }

        /// <summary>
        /// When overridden in a derived class, used child providers to save changes to children.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="existing">The existing.</param>
        /// <returns>Task.</returns>
        protected virtual Task SaveChildren(TEntity entity, TEntity existing, bool deferCommit = false)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// When Overridden in a derived class, uses a child provider to deleted the specified entities children.
        /// </summary>
        /// <param name="existing">The existing.</param>
        /// <returns>Task.</returns>
        protected virtual Task DeleteChildren(TEntity existing, bool deferCommit = false)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Class ManagerBase.
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    public abstract class ManagerBase<TId, TEntity, TSummary>
        : ManagerBase<TId, TEntity, TSummary, SearchCriteria>, IManager<TId, TEntity, TSummary, SearchCriteria>

        where TEntity : EntityBase<TId>
       where TSummary : Entity

    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerBase" /> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="validationManager">The validation manager.</param>
        /// <param name="workflowManager">The workflow manager.</param>
        protected ManagerBase(
            IMapperRegistry mapper,
            IProvider<TId, TEntity, TSummary> provider,
            IValidationManager validationManager,
            IWorkflowManager workflowManager = null) : base(mapper, provider, validationManager, workflowManager) { }
    }


    /// <summary>
    /// Class ManagerBase.
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="SearchCriteria" />
    /// <seealso cref="SearchCriteria" />
    public abstract class ManagerBase<TId, TEntity>
       : ManagerBase<TId, TEntity, TEntity, SearchCriteria>, IManager<TId, TEntity, TEntity, SearchCriteria>
       where TEntity : EntityBase<TId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerBase{TId, TEntity}" /> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="validationManager">The validation manager.</param>
        /// <param name="workflowManager">The workflow manager.</param>
        protected ManagerBase(
            IMapperRegistry mapper,
            IProvider<TId, TEntity> provider,
            IValidationManager validationManager,
            IWorkflowManager workflowManager = null) : base(mapper, provider, validationManager, workflowManager) { }
    }

}



