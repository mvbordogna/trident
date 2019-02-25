using System;
using System.ComponentModel.DataAnnotations;
using Trident.Contracts;

namespace Trident.Domain
{

    /// <summary>
    /// Supply an abstract implementation for all entities in using the Reference Architecture
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Key]
        public object Id { get; set; }
       
    }

    /// <summary>
    /// Class EntityBase.
    /// Implements the <see cref="Trident.Domain.Entity" />
    /// Implements the <see cref="Trident.Contracts.IHaveId{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Trident.Domain.Entity" />
    /// <seealso cref="Trident.Contracts.IHaveId{T}" />
    public abstract class EntityBase<T> : Entity, IHaveId<T>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Key]
        public new T Id
        {
            get
            {
                return base.Id != null ? (T)base.Id : default(T);
            }
            set
            {
                base.Id = value;
            }
        }

        /// <summary>
        /// Gets the primary identifier for this instance.
        /// </summary>
        /// <returns>T.</returns>
        public T GetId()
        {
            return Id;
        }

        /// <summary>
        /// Gets the primary identifier for this instance.
        /// </summary>
        /// <returns>T.</returns>
        T IHaveId<T>.GetId()
        {
            return Id;
        }
    }

    /// <summary>
    /// Class EntityGuidBase.
    /// Implements the <see cref="Trident.Domain.EntityBase{System.Guid}" />
    /// </summary>
    /// <seealso cref="Trident.Domain.EntityBase{System.Guid}" />
    public abstract class EntityGuidBase : EntityBase<Guid>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Key]
        public new Guid Id
        {
            get
            {
                return base.Id != default(Guid) ? base.Id : base.Id = Guid.NewGuid();
            }
            set
            {
                base.Id = value;
            }
        }
    }

    /// <summary>
    /// Class EntityIntBase.
    /// Implements the <see cref="Trident.Domain.EntityBase{System.Int32}" />
    /// </summary>
    /// <seealso cref="Trident.Domain.EntityBase{System.Int32}" />
    public abstract class EntityIntBase : EntityBase<int>
    {

    }

}
