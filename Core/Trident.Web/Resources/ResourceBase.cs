using System;

namespace Trident.Web.Resources
{

    public abstract class ResourceBase
    {
        public Object Id { get; set; }
    }

    public abstract class ResourceBase<TId> : ResourceBase
    {
        public virtual new TId Id
        {
            get
            {
                return (base.Id != null) ? (TId)base.Id : default(TId);
            }
            set
            {

                base.Id = value;
            }
        }     
    }

    public abstract class TridentDualIdentifierResourceBase<TId> : ResourceBase<TId>
    {
        public new TId Id
        {
            get
            {
                return (base.Id != null) ? (TId)base.Id : default(TId);
            }
            set
            {

                base.Id = value;
            }
        }

        public Guid Identifier { get; set; }
    }

    public abstract class ResourceGuidBase : ResourceBase<Guid>
    {
        public virtual new Guid Id
        {
            get
            {
                return (base.Id != Guid.Empty) ? base.Id : Guid.NewGuid();
            }
            set
            {

                base.Id = value;
            }
        }
    }
}
