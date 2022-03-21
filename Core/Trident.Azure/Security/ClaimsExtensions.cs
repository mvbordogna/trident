using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Trident.Azure.Security
{
    public static class ClaimsExtensions
    {
        public static string GetObjectId(this IEnumerable<Claim> claims)
        {
            return claims.GetClaimValue(ClaimConstants.Types.ObjectId);
        }

        public static string GetEnterpriseUserId(this IEnumerable<Claim> claims)
        {
            return claims.GetClaimValue(ClaimConstants.Types.EnterpriseUserId);
        }

        public static string GetAppId(this IEnumerable<Claim> claims)
        {
            return claims.GetClaimValue(ClaimConstants.Types.AppId);
        }

        public static string GetDisplayName(this IEnumerable<Claim> claims)
        {
            return claims.GetClaimValue(ClaimConstants.Types.DisplayName);
        }

        public static IEnumerable<string> GetRoles(this IEnumerable<Claim> claims)
        {
            return claims.GetClaimValues(ClaimConstants.Types.Role);
        }

        public static IEnumerable<string> GetExternalSystemAccess(this IEnumerable<Claim> claims)
        {
            return claims.GetClaimValues(ClaimConstants.Types.ExternalSystemAccess);
        }

        public static IEnumerable<Guid> GetAgencyIds(this IEnumerable<Claim> claims)
        {
            var accountIds = claims.GetClaimValues(ClaimConstants.Types.AccountId);

            var agencyIds = accountIds
                .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty);

            return agencyIds;
        }

        public static string GetClaimValue(this IEnumerable<Claim> claims, string claimType)
        {
            if (string.IsNullOrWhiteSpace(claimType))
                throw new ArgumentNullException(nameof(claimType));

            var claimValues = claims.GetClaimValues(claimType);
            var claim = claimValues.FirstOrDefault();

            return claim ?? string.Empty;
        }

        public static IEnumerable<string> GetClaimValues(this IEnumerable<Claim> claims, string claimType)
        {
            if (string.IsNullOrWhiteSpace(claimType))
                throw new ArgumentNullException(nameof(claimType));

            var claimsList = claims as IList<Claim> ?? claims.ToList();

            if (!claimsList.Any())
            {
                return Enumerable.Empty<string>();
            }

            var claimValues = claimsList
                .Where(c => c.Type == claimType)
                .Select(s => s.Value);

            return claimValues;
        }

        public static bool HasAllClaimValues(this IEnumerable<Claim> claims, string claimType, IEnumerable<string> expectedValues)
        {
            var claimValues = claims.GetClaimValues(claimType);
            var hasAll = expectedValues?.All(value => claimValues.Contains(value)) ?? false;
            return hasAll;
        }
        
        public static bool HasAnyClaimValues(this IEnumerable<Claim> claims, string claimType, IEnumerable<string> expectedValues)
        {
            var claimValues = claims.GetClaimValues(claimType);
            var hasAny = expectedValues?.Any(value => claimValues.Contains(value)) ?? false;
            return hasAny;
        }
    }
}
