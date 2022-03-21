using System.Diagnostics.CodeAnalysis;

namespace Trident.Azure.Security
{
    /// <summary>
    /// constants associated with Claims
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ClaimConstants
    {
        /// <summary>
        /// value used for Type to identify a claim in a JWT
        /// </summary>
        public static class Types
        {
            /// <summary>
            /// uniquely identifies a user by object id
            /// </summary>
            /// <remarks>
            /// e.g. Azure AD B2C ObjectId of user
            /// </remarks>
            public const string ObjectId = "oid";

            /// <summary>
            /// user display name
            /// </summary>
            public const string DisplayName = "name";

            /// <summary>
            /// user first name
            /// </summary>
            public const string FirstName = "given_name";

            /// <summary>
            /// user last name
            /// </summary>
            public const string LastName = "family_name";

            /// <summary>
            /// identity provider of token
            /// </summary>
            public const string IdentityProvider = "idp";

            /// <summary>
            /// issuer of a token
            /// </summary>
            public const string Issuer = "iss";

            /// <summary>
            /// any roles user is assigned to
            /// </summary>
            public const string Role = "role";

            /// <summary>
            /// any accounts user is associated with
            /// </summary>
            public const string AccountId = "accountId";

            /// <summary>
            /// source of the token
            /// </summary>
            public const string TokenSource = "source";

            /// <summary>
            /// any employer groups user is associated with
            /// </summary>
            public const string EmployerGroup = "employerGroup";

            /// <summary>
            /// any EAS user is associated with
            /// </summary>
            public const string Eas = "eas";

            /// <summary>
            /// represents app id if authentication was performed using app registration
            /// </summary>
            public const string AppId = "appid";

            /// <summary>
            /// id that identifies the user within the enterprise
            /// </summary>
            public static string EnterpriseUserId = "enterpriseUserId";

            /// <summary>
            /// claim used to identify access user has for templates
            /// </summary>
            public static string Template = "template";

            /// <summary>
            /// claim used to identify ExternalSystemAccess permissions for user
            /// </summary>
            public static string ExternalSystemAccess = "externalSystemAccess";

            /// <summary>
            /// claim used to identify access user has for managing or retrieving content
            /// </summary>
            public static string ContentAccess = "contentAccess";
        }

        public static class Values
        {
            /// <summary>
            /// value used in claims to indicate token source came from STS
            /// </summary>
            public const string StsSourceValue = "EntitlementSTS";

            /// <summary>
            /// used in some claims to concat multi values together
            /// </summary>
            public const string MultiValueSeparator = "|";
        }
    }
}
