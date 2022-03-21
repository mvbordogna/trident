using Newtonsoft.Json;

namespace Trident.Cosmos.Contracts
{
    public class CosmosCollection
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("_self")]
        public string Self { get; set; }

        [JsonProperty("_etag")]
        public string ETag { get; set; }      

        [JsonProperty("_docs")]
        public string Docs { get; set; }

        [JsonProperty("_sprocs")]
        public string Sprocs { get; set; }

        [JsonProperty("_triggers")]
        public string Triggers { get; set; }

        [JsonProperty("_udfs")]
        public string Udfs { get; set; }

        [JsonProperty("_conflicts")]
        public string Conflicts { get; set; }

        [JsonProperty("_ts")]
        public string TS { get; set; }

        [JsonProperty("indexingPolicy")]
        public IndexingPolicy IndexingPolicy { get; set; }

        [JsonProperty("partitionKey")]
        public PartitionKey PartitionKey { get; set; }

        [JsonProperty("conflictResolutionPolicy")]
        public ConflictResolutionPolicy ConflictResolutionPolicy { get; set; }

        [JsonProperty("geospatialConfig")]
        public GeospatialConfig GeospatialConfig { get; set; }
    }
}
        