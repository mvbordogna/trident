using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Trident.Api.Search
{
    public class CompareModel
    {
        public object Value { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CompareOperators Operator { get; set; }
        public bool IgnoreCase { get; set; } = true;
    }

}
