using Newtonsoft.Json;

namespace Trident.UI.Blazor.Logging.AppInsights
{
    public class PageViewPerformanceTelemetry
    {
        /// <summary>
        /// The name of the page. Defaults to the document title.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// a relative or absolute URL that identifies the page or other item. Defaults to the window location.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// Performance total in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff". This is total duration in timespan format.
        /// </summary>
        [JsonProperty("perfTotal")]
        public string PerfTotal { get; set; }

        /// <summary>
        /// Performance total in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff". This represents the total page load time.
        /// </summary>
        [JsonProperty("duration")]
        public string Duration { get; set; }

        /// <summary>
        /// Sent request time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff.
        /// </summary>
        [JsonProperty("networkConnect")]
        public string NetworkConnect { get; set; }

        /// <summary>
        /// Sent request time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
        /// </summary>
        [JsonProperty("sentRequest")]
        public string SentRequest { get; set; }

        /// <summary>
        /// Received response time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff.
        /// </summary>
        [JsonProperty("receivedResponse")]
        public string ReceivedResponse { get; set; }

        /// <summary>
        /// DOM processing time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
        /// </summary>
        [JsonProperty("domProcessing")]
        public string DomProcessing { get; set; }
    }
}
