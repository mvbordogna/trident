using Trident.Configuration;

namespace Trident.UI.Client.Contracts.Models
{
    public interface IHttpConfiguration : ICoreConfiguration
    {
        public string ServiceName { get; }
        public string BaseUrl { get; }
        public string Scopes { get; }
        public string UserFlow { get; }
    }
}
