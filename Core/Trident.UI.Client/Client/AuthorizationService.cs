using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Trident.UI.Blazor.Contracts.Services;

namespace Trident.UI.Client
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ILogger<AuthorizationService> _logger;

        private static readonly object _lock = new object();

        private Dictionary<string, string> _userPermissions = null;

        public AuthorizationService(ILogger<AuthorizationService> logger)
        {
            _logger = logger;
        }

        public string GetClaim(ClaimsPrincipal user, string name)
        {
            if (!IsUserAuthenticated(user))
            {
                return String.Empty;
            }

            return user?.Claims.FirstOrDefault(x => x.Type == name)?.Value ?? string.Empty;
        }

        public bool HasClaim(ClaimsPrincipal user, string name, string operations)
        {

            if (!IsUserAuthenticated(user))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(operations))
            {
                throw new ArgumentException($"'{nameof(operations)}' cannot be null or whitespace.", nameof(operations));
            }

            var claim = GetClaim(user, name);
            return claim == operations;
        }

        public bool HasPermission(ClaimsPrincipal user, string name, string operations)
        {
            try
            {
                if (!IsUserAuthenticated(user))
                {
                    return false;
                }

                var permissions = GetPermissionsFromJwt(user);

                permissions = new Dictionary<string, string>(permissions, StringComparer.OrdinalIgnoreCase);
                var exists = permissions.TryGetValue(name, out var op);
                if (!exists)
                    return false;

                return HasOperation(operations, op);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking for claim '{name}:{operations}': {ex.Message}");
                throw;
            }
        }

        private bool IsUserAuthenticated(ClaimsPrincipal user)
        {
            return user?.Identity.IsAuthenticated ?? false;
        }

        private static bool HasOperation(string source, string target)
        {
            var result = source.All(s => target.Contains(s, StringComparison.OrdinalIgnoreCase));
            return result;
        }

        private Dictionary<string, string> GetPermissionsFromJwt(ClaimsPrincipal user)
        {
            if (user?.Identity.IsAuthenticated ?? false)
            {
                if (this._userPermissions == null)
                {
                    lock (_lock)
                    {
                        if (this._userPermissions == null)
                        {
                            this._userPermissions = GetPermissionsFromJwtImpl(user);
                        }
                    }
                }
            }
            else
            {
                _userPermissions = null;
            }

            return _userPermissions;
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
