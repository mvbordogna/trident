namespace Trident.Contracts.Api
{
    public abstract class ApiModelBase<TId>
    {
        public TId Id { get; set; }
    }
}
