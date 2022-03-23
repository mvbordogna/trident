namespace Trident.UI.Blazor.Logging.AppInsights
{
    public interface IAppInsightsConfig
    {
        string InstrumentationKey { get; }
        string ConnectionString { get; }
        bool DisableFetchTracking { get; }
        bool EnableCorsCorrelation { get; }
        bool EnableRequestHeaderTracking { get; }
        bool EnableResponseHeaderTracking { get; }
    }

    public class AppInsightsConfig : IAppInsightsConfig
    {
        public string InstrumentationKey { get; set; }
        public string ConnectionString { get; set; }

        public bool DisableFetchTracking { get; set; }
        public bool EnableCorsCorrelation { get; set; } 
        public bool EnableRequestHeaderTracking { get; set; }
        public bool EnableResponseHeaderTracking { get; set; }
    }
}
