using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Trident.Contracts;
using Trident.Data;
using Trident.Extensions;

namespace Trident.Domain
{

    /// <summary>
    /// Supply an abstract implementation for all entities in using the Reference Architecture
    /// </summary>
    public abstract class Entity
    {
        protected object id;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Key]
        public object Id { get { return id; } set { id = value; } }

    }

    /// <summary>
    /// Class EntityBase.
    /// Implements the <see cref="TridentOptionsBuilder.Domain.Entity" />
    /// Implements the <see cref="TridentOptionsBuilder.Contracts.IHaveId{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TridentOptionsBuilder.Domain.Entity" />
    /// <seealso cref="TridentOptionsBuilder.Contracts.IHaveId{T}" />
    public abstract class EntityBase<T> : Entity, IHaveId<T>
    {
        public EntityBase()
        {
            id = default(T);
        }

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
    /// Implements the <see cref="TridentOptionsBuilder.Domain.EntityBase{System.Guid}" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Domain.EntityBase{System.Guid}" />
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
    /// Implements the <see cref="TridentOptionsBuilder.Domain.EntityBase{System.Int32}" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Domain.EntityBase{System.Int32}" />
    public abstract class EntityIntBase : EntityBase<int>
    {

    }

    public abstract class EntityLongBase : EntityBase<long>
    {

    }

    public abstract class TridentDualIdenityEntityBase<T> : EntityBase<T>
    {
        public Guid Identifier { get; set; }
    }

    public abstract class DocumentDbEntityBase : EntityBase<Guid>
    {

        private string _documentType;
        private string _entityType;

        protected DocumentDbEntityBase()
        {
            var thisType = this.GetType();
            var containerAttr = thisType.GetCustomAttribute<ContainerAttribute>();
            var discriminatorAttr = thisType.GetCustomAttribute<DiscriminatorAttribute>();
                     
            containerAttr.GuardIsNotNull(nameof(containerAttr), "Container Attribute is Require for enitties stored in Cosmos DB");
            discriminatorAttr.GuardIsNotNull(nameof(discriminatorAttr), "Discriminator Attribute is Require for enitties stored in Cosmos DB");

            if (containerAttr != null)
            {
                containerAttr.Name.GuardIsNotNullOrWhitespace($"{nameof(containerAttr)}.{nameof(containerAttr.PartitionKey)}");
                containerAttr.Name.GuardIsNotNullOrWhitespace($"{nameof(containerAttr)}.{nameof(containerAttr.PartitionKeyValue)}");
                this._documentType = containerAttr.PartitionKeyValue;
            }

            if (discriminatorAttr != null)
            {
                discriminatorAttr.Property.GuardIsNotNullOrWhitespace($"{nameof(discriminatorAttr)}.{nameof(discriminatorAttr.Property)}");
                discriminatorAttr.Value.GuardIsNotNullOrWhitespace($"{nameof(discriminatorAttr)}.{nameof(discriminatorAttr.Value)}");
                this._entityType = discriminatorAttr.Value;
            }
        }


        public string DocumentType { get { return _documentType; } protected set { } }
        public string EntityType { get { return _entityType; } protected set { } }
    }



}
