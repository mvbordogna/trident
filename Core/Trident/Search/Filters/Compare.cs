using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Trident.Search
{
    public class Compare
    {
        public object Value { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CompareOperators Operator { get; set; }
        public bool IgnoreCase { get; set; } = true;
    }

}
