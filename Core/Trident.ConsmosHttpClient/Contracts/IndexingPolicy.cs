using Newtonsoft.Json;

namespace Trident.Cosmos.Contracts
{
    public class IndexingPolicy
    {
        [JsonProperty("indexingMode")]
        public string IndexingMode { get; set; }

        [JsonProperty("automatic")]
        public string Automatic { get; set; }

        [JsonProperty("includedPaths")]
        public PathPattern[] IncludedPaths { get; set; }

        [JsonProperty("excludedPaths")]
        public PathPattern[] ExcludedPaths { get; set; }
    }
}
        