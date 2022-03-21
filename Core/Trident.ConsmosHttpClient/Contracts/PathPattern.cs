using Newtonsoft.Json;

namespace Trident.Cosmos.Contracts
{
    public class PathPattern
    {
        [JsonProperty("path")]
        public string Path { get; set; }        
    }
}
        