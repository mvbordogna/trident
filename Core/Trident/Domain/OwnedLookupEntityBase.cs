using System;
using System.ComponentModel.DataAnnotations;

namespace Trident.Domain
{
    public abstract class OwnedLookupEntityBase<Tid>
    {
        [Key]
        protected Tid id { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public abstract class OwnedEntityBase<Tid>
    {
        [Key]
        protected Tid id { get; set; }
        public Tid Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
    }
}
