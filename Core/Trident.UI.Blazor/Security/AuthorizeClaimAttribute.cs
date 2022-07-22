using Microsoft.AspNetCore.Authorization;
using System;

namespace Trident.UI.Blazor.Security
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class AuthorizeClaimAttribute : AuthorizeAttribute
    {

        // This is a positional argument
        public AuthorizeClaimAttribute(string claimType, string claimValue)
        {
            ClaimType = claimType;
            ClaimValue = claimValue;
        }

        public string ClaimType { get; }

        public string ClaimValue { get; }
    }
}
