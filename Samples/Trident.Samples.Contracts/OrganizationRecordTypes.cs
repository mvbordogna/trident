using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Trident.Samples.Contracts
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrganizationRecordTypes
    {
        All = 0,
        Organization,
        Department,
        Error
    }
}
