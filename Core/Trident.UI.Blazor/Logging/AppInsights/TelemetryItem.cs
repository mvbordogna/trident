using Newtonsoft.Json;
using System.Collections.Generic;

namespace Trident.UI.Blazor.Logging.AppInsights
{
    public class TelemetryItem
    {
        /// <summary>
        /// CommonSchema Version of this SDK
        /// </summary>
        [JsonProperty("ver")]
        public string Ver { get; set; }

        /// <summary>
        /// Unique name of the telemetry item
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Timestamp when item was sent
        /// </summary>
        [JsonProperty("time")]
        public string Time { get; set; }

        /// <summary>
        /// Identifier of the resource that uniquely identifies which resource data is sent to
        /// </summary>
        [JsonProperty("iKey")]
        public string IKey { get; set; }

        /// <summary>
        /// System context properties of the telemetry item, example: ip address, city etc
        /// </summary>
        [JsonProperty("ext")]
        public Dictionary<string, object> Ext { get; set; }

        /// <summary>
        /// System context property extensions that are not global (not in ctx)
        /// </summary>
        [JsonProperty("tags")]
        public Dictionary<string, object> Tags { get; set; }

        /// <summary>
        /// Custom data
        /// </summary>
        [JsonProperty("data")]
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// Telemetry type used for part B
        /// </summary>
        [JsonProperty("baseType")]
        public string BaseType { get; set; }

        /// <summary>
        /// Based on schema for part B
        /// </summary>
        [JsonProperty("baseData")]
        public Dictionary<string, object> BaseData { get; set; }
    }
}
