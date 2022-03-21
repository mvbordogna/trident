using Newtonsoft.Json;

namespace Trident.Cosmos.Contracts
{
    public class ConflictResolutionPolicy
    {
        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("conflictResolutionPath")]
        public string ConflictResolutionPath { get; set; }

        [JsonProperty("conflictResolutionProcedure")]
        public string ConflictResolutionProcedure { get; set; }
    }
}
        