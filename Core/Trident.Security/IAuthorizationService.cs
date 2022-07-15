using System.Security.Claims;
using System.Threading.Tasks;
using Trident.Contracts;

namespace Trident.Security
{
	public interface IAuthorizationService : IService
	{
		bool HasPermission(ClaimsPrincipal user, string name, string operations);

		bool HasClaim(ClaimsPrincipal user, string name, string operations);

		string GetClaim(ClaimsPrincipal user, string name);

		Task<ClaimsPrincipal> ValidateToken(string token);
	}
}
