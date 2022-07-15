namespace Trident.Security
{
    internal class TokenValidation
    {
        public string ValidIssuer { get; set; }

        public string ValidAudience { get; set; }

        public string IssuerSigningKey { get; set; }
    }
}
