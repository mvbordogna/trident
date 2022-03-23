namespace Trident.UI.Client.Contracts.Models
{
    public class ModelBase<TId>
    {
        public virtual TId Id { get; set; }
    }

    public class BasePartneredModel : GuidModelBase
    {
        public string PartnerId { get; set; }
    }
}
