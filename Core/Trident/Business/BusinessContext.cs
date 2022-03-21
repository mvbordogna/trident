using Trident.Extensions;
using System;
using System.Collections.Generic;

namespace Trident.Business
{

    /// <summary>
    /// Class WorkflowContext.
    /// </summary>
    public abstract class BusinessContext
    {
        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public virtual object Target { get; protected set; }

        /// <summary>
        /// Gets or sets the original.
        /// </summary>
        /// <value>The original.</value>
        public object Original { get; protected set; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>The operation.</value>
        public Operation Operation { get; set; }

    }

    /// <summary>
    /// Class WorkflowContext.
    /// Implements the <see cref="TridentOptionsBuilder.Workflow.WorkflowContext" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TridentOptionsBuilder.Workflow.WorkflowContext" />
    public class BusinessContext<T> : BusinessContext
        where T : class
    {

        private const string __customOperation = nameof(__customOperation);

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessContext{T}" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="original">The original.</param>
        /// <param name="contextBag">The context bag.</param>
        public BusinessContext(T target, T original = null, IDictionary<string, Object> contextBag = null)
        {
            this.Target = target;
            this.Original = original;
            this.ContextBag = contextBag ?? new Dictionary<string, Object>();
        }

        /// <summary>
        /// Gets or sets the target entity for validation.
        /// </summary>
        /// <value>The target.</value>
        public new T Target
        {
            get
            {
                return base.Target as T;
            }
            protected set
            {
                base.Target = value;
            }
        }

        /// <summary>
        /// Gets the context bag.
        /// </summary>
        /// <value>The context bag.</value>
        public IDictionary<string, object> ContextBag { get; }


        /// <summary>
        /// Gets or sets the Existing persisted entity.
        /// </summary>
        /// <value>The target.</value>
        public new T Original
        {
            get
            {
                return base.Original as T;
            }
            protected set
            {
                base.Original = value;
            }
        }

        /// <summary>
        /// Gets the context bag item or default.
        /// </summary>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>TValue.</returns>
        public TValue GetContextBagItemOrDefault<TValue>(string key, TValue defaultValue = default(TValue))
        {
            var val = this.ContextBag.GetValueOrDefault(key, defaultValue);
            if (val != null)
            {
                return (val is IConvertible)
                     ? val.ChangeType<TValue>()
                     : (TValue)val;
            }

            return defaultValue;
        }
        
        public void SetCustomOperation(object operation)
        {
            this.ContextBag[__customOperation] = operation;
        }

        public TOperation GetCustomOperation<TOperation>()
        {
            return GetContextBagItemOrDefault<TOperation>(__customOperation);
        }

    }
}
