namespace Trident.Contracts.Api.Client
{
    public interface IHttpNamedServiceBase : IHttpServiceBase       
    {
        string HttpServiceName { get; }
    }
}
