using Newtonsoft.Json;

namespace Trident.Cosmos.Contracts
{
    public class PartitionKey
    {
        [JsonProperty("paths")]
        public string[] Paths { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }
    }
}
        