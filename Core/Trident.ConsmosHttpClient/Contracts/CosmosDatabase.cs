using Newtonsoft.Json;

namespace Trident.Cosmos.Contracts
{
    public class CosmosDatabase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("_self")]
        public string Self { get; set; }

        [JsonProperty("dbs")]
        public string DBS { get; set; }

        [JsonProperty("_etag")]
        public string ETag { get; set; }

        [JsonProperty("_colls")]
        public string Colls { get; set; }

        [JsonProperty("_users")]
        public string Users { get; set; }

        [JsonProperty("_ts")]
        public string TS { get; set; }
    }
}
        