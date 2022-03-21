using System;

namespace Trident.Azure.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ClaimsAuthorizeAttribute : Attribute
    {
        public ClaimsAuthorizeAttribute(string claimType, string claimValue)
        {
            this.Claim = new ClaimEntity(claimType, claimValue);
        }
        public ClaimEntity Claim { get; }

        public sealed class ClaimEntity
        {
            internal ClaimEntity(string type, string value)
            {
                this.Type = type;
                this.Value = value;
            }

            public string Type { get; }
            public string Value { get; }
        }
    }
}
