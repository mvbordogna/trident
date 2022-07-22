using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Trident.UI.Blazor.Security
{
    public class ClaimsRequirement : AuthorizationHandler<ClaimsRequirement>, IAuthorizationRequirement
    {
        private Dictionary<string, string> _scopedUserPermissions = null;

        public Guid InstanceId { get; } = Guid.NewGuid();

        public ClaimsRequirement() { }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimsRequirement requirement)
        {
            try
            {
                if (context.User != null && context.User.Identity.IsAuthenticated)
                {
                    if (context.Resource is ResourceContext secContext)
                    {
                        bool meetsClaimRequirements = true;
                        var claims = secContext.GetClaimRequirements();

                        if (claims != null && claims.Any())
                        {
                            meetsClaimRequirements &= claims.Any(x => HasClaim(
                                context.User,
                                x.ClaimType ?? string.Empty,
                                x.ClaimValue ?? string.Empty)
                            );
                        }

                        if (!meetsClaimRequirements)
                        {
                            context.Fail();
                        }
                    }

                    context.Succeed(this);
                }
                else
                {
                    var anonRequirementHandler = new DenyAnonymousAuthorizationRequirement();
                    await anonRequirementHandler.HandleAsync(context);
                }
            }
            catch (Exception ex)
            {
                var e = ex;
            }
        }

        public Task<string> GetClaim(ClaimsPrincipal user, string name)
        {
            string result = string.Empty;
            var claim = user?.Claims.FirstOrDefault(x => x.Type == name);
            result = claim?.Value ?? string.Empty;
            return Task.FromResult(result);
        }

        private bool HasClaim(ClaimsPrincipal user, string name, string operations)
        {
            try
            {
                if (user != null && user.Identity.IsAuthenticated)
                {
                    if (_scopedUserPermissions == null)
                    {
                        _scopedUserPermissions = GetPermissionsFromJwtImpl(user);
                    }


                    var permissions = new Dictionary<string, string>(_scopedUserPermissions, StringComparer.OrdinalIgnoreCase);
                    var exists = permissions.TryGetValue(name, out var op);
                    if (!exists)
                        return false;

                    var hasOperation = HasOperation(operations, op);
                    return hasOperation;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool HasOperation(string source, string target)
        {
            var result = source.All(s => target.Contains(s, StringComparison.OrdinalIgnoreCase));
            return result;
        }

        private Dictionary<string, string> GetPermissionsFromJwtImpl(ClaimsPrincipal user)
        {
            var permissionsClaim = user.FindFirst("permissions");
            if (permissionsClaim?.Value != null)
            {
                var result = DecodePermissions(permissionsClaim.Value);
                return result;
            }

            return new Dictionary<string, string>();
        }

        private static Dictionary<string, string> DecodePermissions(string encoded)
        {
            var bytes = Convert.FromBase64String(encoded);
            var decoded = Encoding.UTF8.GetString(bytes);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(decoded);
            return dict ?? new Dictionary<string, string>();
        }
    }
}
