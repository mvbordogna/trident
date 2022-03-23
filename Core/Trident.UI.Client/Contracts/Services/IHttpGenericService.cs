using System.Threading.Tasks;
using Trident.Contracts.Api.Client;
using Trident.UI.Client.Contracts.Models;

namespace Trident.UI.Blazor.Contracts.Services
{
    public interface IHttpGenericService : Trident.Contracts.Api.Client.IHttpServiceBase
    {
        Task<Response<T>> SendRequest<T>(string service, string method, string route, object data = null) where T : class;
    }
}
