using System.Security.Claims;
using Trident.Contracts.Api.Client;

namespace Trident.UI.Blazor.Contracts.Services
{
    public interface IAuthorizationService : IServiceProxy
    {
        bool HasPermission(ClaimsPrincipal user, string name, string operations);

        bool HasClaim(ClaimsPrincipal user, string name, string operations);

        string GetClaim(ClaimsPrincipal user, string name);
    }
}
