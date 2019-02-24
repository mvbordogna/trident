﻿using Trident.Business;
using Trident.Domain;
using Trident.Extensions;
using System;
using System.Collections.Generic;

namespace Trident.Workflow
{

    /// <summary>
    /// Class ValidationContext.
    /// </summary>
    public abstract class WorkflowContext
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
    /// Class ValidationContext.
    /// Implements the <see cref="Trident.Workflow.WorkflowContext" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Trident.Workflow.WorkflowContext" />
    public class WorkflowContext<T>: WorkflowContext
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowContext{T}" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="original">The original.</param>
        /// <param name="contextBag">The context bag.</param>
        public WorkflowContext(T target, T original = null, IDictionary<string, Object> contextBag = null)
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
    }
}
