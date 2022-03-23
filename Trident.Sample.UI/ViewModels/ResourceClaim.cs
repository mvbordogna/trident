namespace Trident.Sample.UI.ViewModels
{
    public class ResourceClaim
    {


        public ResourceClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; }
        public string Value { get; }
    }
}
