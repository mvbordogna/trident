using Microsoft.JSInterop;
using Microsoft.JSInterop.WebAssembly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trident.UI.Blazor.Logging.AppInsights
{
    public class ApplicationInsights : IApplicationInsights
    {    
        private readonly IAppInsightsConfig _config;
        private IJSRuntime _jsRuntime = null;

        
        public bool EnableAutoRouteTracking { get; set; }

        public ApplicationInsights(IAppInsightsConfig config, IJSRuntime jSRuntime)
        {
            _config = config;
            _jsRuntime = jSRuntime;
            InitBlazorAppInsights();
        }

        public void InitBlazorAppInsights()
        {
            if (_config != null 
                && (!string.IsNullOrWhiteSpace(_config.ConnectionString) || !string.IsNullOrWhiteSpace(_config.InstrumentationKey)))
            {
                if (!string.IsNullOrWhiteSpace(_config.ConnectionString)  )
                {
                    this.SetConnectionString(_config.ConnectionString);                   
                }

                if (!string.IsNullOrWhiteSpace(_config.InstrumentationKey))
                {
                    this.SetInstrumentationKey(_config.InstrumentationKey);
                }

                this.SetDisableFetchTracking(_config.DisableFetchTracking);
                this.SetEnableCorsCorrelation(_config.EnableCorsCorrelation);
                this.SetEnableRequestHeaderTracking(_config.EnableRequestHeaderTracking);
                this.SetEnableResponseHeaderTracking(_config.EnableResponseHeaderTracking);
                this.LoadAppInsights();
            }
            else
            {
                WriteConsole($"On Config injected into {nameof(ApplicationInsights)}, App Insights logging will be disabled");
            }                  
        }

        
        public async Task TrackPageView(string name = null, string uri = null, string refUri = null, string pageType = null, bool? isLoggedIn = null, Dictionary<string, object> properties = null)
            => await _jsRuntime.InvokeVoidAsync("appInsights.trackPageView", new { name, uri, refUri, pageType, isLoggedIn }, properties!);

        
        public async Task TrackEvent(string name, Dictionary<string, object> properties = null)
            => await _jsRuntime.InvokeVoidAsync("appInsights.trackEvent", new { name }, properties!);

        
        public async Task TrackTrace(string message, SeverityLevel? severityLevel = null, Dictionary<string, object> properties = null)
            => await _jsRuntime.InvokeVoidAsync("appInsights.trackTrace", new { message, severityLevel }, properties!);

        
        public async Task TrackException(Error exception, string id = null, SeverityLevel? severityLevel = null, Dictionary<string, object> properties = null)
            => await _jsRuntime.InvokeVoidAsync("appInsights.trackException", new { id, exception, severityLevel }, properties!);

        
        public async Task StartTrackPage(string name = null)
            => await _jsRuntime.InvokeVoidAsync("appInsights.startTrackPage", name!);

        
        public async Task StopTrackPage(string name = null, string url = null, Dictionary<string, string> properties = null, Dictionary<string, decimal> measurements = null)
            => await _jsRuntime.InvokeVoidAsync("appInsights.stopTrackPage", name!, url!, properties!, measurements!);

        
        public async Task TrackMetric(string name, double average, double? sampleCount = null, double? min = null, double? max = null, Dictionary<string, object> properties = null)
            => await _jsRuntime.InvokeVoidAsync("appInsights.trackMetric", new { name, average, sampleCount, min, max }, properties!);

        
        public async Task TrackDependencyData(string id, string name, decimal? duration = null, bool? success = null, DateTime? startTime = null, int? responseCode = null, string correlationContext = null, string type = null, string data = null, string target = null)
            => await _jsRuntime.InvokeVoidAsync("blazorAppInsights.trackDependencyData", new { id, name, duration, success, startTime = startTime?.ToString("yyyy-MM-ddTHH:mm:ss"), responseCode, correlationContext, type, data, target });

        
        public async Task Flush(bool? async = true)
            => await _jsRuntime.InvokeVoidAsync("appInsights.flush", async!);

        
        public async Task ClearAuthenticatedUserContext()
            => await _jsRuntime.InvokeVoidAsync("appInsights.clearAuthenticatedUserContext");

        
        public async Task SetAuthenticatedUserContext(string authenticatedUserId, string accountId = null, bool storeInCookie = false)
            => await _jsRuntime.InvokeVoidAsync("appInsights.setAuthenticatedUserContext", authenticatedUserId, accountId!, storeInCookie);

        
        public async Task AddTelemetryInitializer(TelemetryItem telemetryItem)
            => await _jsRuntime.InvokeVoidAsync("blazorAppInsights.addTelemetryInitializer", telemetryItem);

        
        public async Task TrackPageViewPerformance(PageViewPerformanceTelemetry pageViewPerformance)
            => await _jsRuntime.InvokeVoidAsync("appInsights.trackPageViewPerformance", pageViewPerformance);

        
        public async Task StartTrackEvent(string name)
            => await _jsRuntime.InvokeVoidAsync("appInsights.startTrackEvent", name);

        
        public async Task StopTrackEvent(string name, Dictionary<string, string> properties = null, Dictionary<string, decimal> measurements = null)
            => await _jsRuntime.InvokeVoidAsync("appInsights.stopTrackEvent", name, properties!, measurements!);

        
        public void SetInstrumentationKey(string key)
            => ((WebAssemblyJSRuntime)_jsRuntime).InvokeVoid("blazorAppInsights.setInstrumentationKey", key);

        public void SetConnectionString(string connStr)
        {
            var wJSruntime = (WebAssemblyJSRuntime)_jsRuntime;
            wJSruntime.InvokeVoid("blazorAppInsights.setConnectionString", connStr);
        }
        public void SetDisableFetchTracking(bool disabled)
       => ((WebAssemblyJSRuntime)_jsRuntime).InvokeVoid("blazorAppInsights.setDisableFetchTracking", disabled);
        public void SetEnableCorsCorrelation(bool enabled)
       => ((WebAssemblyJSRuntime)_jsRuntime).InvokeVoid("blazorAppInsights.setEnableCorsCorrelation", enabled);
        public void SetEnableRequestHeaderTracking(bool enabled)
       => ((WebAssemblyJSRuntime)_jsRuntime).InvokeVoid("blazorAppInsights.setEnableRequestHeaderTracking", enabled);
        public void SetEnableResponseHeaderTracking(bool enabled)
       => ((WebAssemblyJSRuntime)_jsRuntime).InvokeVoid("blazorAppInsights.setEnableResponseHeaderTracking", enabled);

        private void WriteConsole(string message)
         => ((WebAssemblyJSRuntime)_jsRuntime).InvokeVoid("console.warn", message);




        public void LoadAppInsights()
        {
            ((WebAssemblyJSRuntime)_jsRuntime).InvokeVoid("blazorAppInsights.loadAppInsights");
        }
    }
}
