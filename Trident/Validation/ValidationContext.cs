using Trident.Business;
using Trident.Domain;
using Trident.Extensions;
using System;
using System.Collections.Generic;

namespace Trident.Validation
{
    /// <summary>
    /// Class ValidationContext.
    /// </summary>
    public abstract class ValidationContext
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

        /// <summary>
        /// Gets or sets the context bag.
        /// </summary>
        /// <value>The context bag.</value>
        public IDictionary<string, object> ContextBag { get; protected set; }
    }

    /// <summary>
    /// Class ValidationContext.
    /// Implements the <see cref="Trident.Validation.ValidationContext" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Trident.Validation.ValidationContext" />
    public class ValidationContext<T>: ValidationContext
        where T : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext{T}" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="original">The original.</param>
        /// <param name="contextBag">A bag of custom values that can be check in in validation rules.</param>
        public ValidationContext(T target, T original = null, IDictionary<string, object> contextBag = null) { 
            this.Target = target;
            this.Original = original;
            this.ContextBag = contextBag ?? new Dictionary<string, object>();
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


    }
}
