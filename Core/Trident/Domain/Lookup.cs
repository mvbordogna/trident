using System.ComponentModel.DataAnnotations;

namespace Trident.Domain
{
    /// <summary>
    /// Supply an abstract implementation for all entities in using the Reference Architecture
    /// </summary>
    public class Lookup
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Key]
        public object Id { get; set; }

        public string Display { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Lookup<T> : Lookup
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
    }
}
