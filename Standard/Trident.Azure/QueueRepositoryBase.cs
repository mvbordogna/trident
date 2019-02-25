using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Trident.Data.Contracts;
using Trident.Domain;
using System.Threading.Tasks;

namespace Trident.Azure
{
    public abstract class QueueRepositoryBase<TEntity> : AzureStorageRepositoryBase<CloudQueueClient>,
         IQueueRepository<TEntity>
         where TEntity : QueueEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueRepositoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        protected QueueRepositoryBase(IAbstractContextFactory abstractContextFactory)
            : base(abstractContextFactory.Create<CloudQueueClient>(typeof(TEntity)))
        {
        }
        /// <summary>
        /// Gets the name of the queue.
        /// </summary>
        /// <value>The name of the queue.</value>
        protected abstract string QueueName { get; }

        public async Task<bool> Exists()
        {
            var queue = base.ClientContext.GetQueueReference(QueueName);
            return await queue.ExistsAsync();
        }

        public async Task<bool> CreateIfNotExists()
        {        
            var queue = base.ClientContext.GetQueueReference(QueueName);
            return await queue.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// Enqueues the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task.</returns>
        public virtual async Task Enqueue(TEntity entity)
        {
            await Enqueue(JsonConvert.SerializeObject(entity));
        }

        public async Task Enqueue(TEntity entity, bool createQueueIfNotExists)
        {
            if (createQueueIfNotExists) await CreateIfNotExists();
            await Enqueue(entity);
        }

        /// <summary>
        /// Enqueues the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        protected async Task Enqueue(string message)
        {
            var queue = base.ClientContext.GetQueueReference(QueueName);
            await queue.AddMessageAsync(new CloudQueueMessage(message.ToString()));
        }

        /// <summary>
        /// Dequeues the message at the top of the queue
        /// </summary>
        /// <returns></returns>
        public async Task<TEntity> Dequeue()
        {
            var queue = base.ClientContext.GetQueueReference(QueueName);
            var msg = await queue.GetMessageAsync();
            return JsonConvert.DeserializeObject<TEntity>(msg.AsString);
        }
    }
}
