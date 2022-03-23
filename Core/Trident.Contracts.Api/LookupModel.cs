namespace Trident.Contracts.Api
{
    /// <summary>
    /// Supply an abstract implementation for all entities in using the Reference Architecture
    /// </summary>
    public class LookupModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>

        public object Id { get; set; }

        public string Display { get; set; }

        public string PartnerId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LookupModel<T> : LookupModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>

        public new T Id
        {
            get
            {
                return base.Id != null ? (T)base.Id : default;
            }
            set
            {
                base.Id = value;
            }
        }
    }
}
