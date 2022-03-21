using Newtonsoft.Json;

namespace Trident.Cosmos.Contracts
{
    public class GeospatialConfig
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
        