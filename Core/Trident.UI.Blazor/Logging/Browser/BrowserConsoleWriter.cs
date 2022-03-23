using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.JSInterop.WebAssembly;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Trident.UI.Client.Logging;

namespace Trident.UI.Blazor.Logging.Browser
{
    public class BrowserConsoleWriter : IBrowserConsoleWriter
    {
        private readonly OSPlatform WebAssemblyPlatform = OSPlatform.Create("Browser");
        private OSPlatform _supportedPlatform { get; set; }
        protected WebAssemblyJSRuntime JsRuntime { get; }
        public BrowserConsoleWriter(IJSRuntime jSRuntime)
        {
            this.JsRuntime = (WebAssemblyJSRuntime)jSRuntime;
            this._supportedPlatform = WebAssemblyPlatform;
        }

        public void Write(LogLevel logLevel, string formattedMessage)
        {
            var result = RuntimeInformation.OSDescription;
            if (RuntimeInformation.IsOSPlatform(_supportedPlatform))
            {

                switch (logLevel)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                        // Although https://console.spec.whatwg.org/#loglevel-severity claims that
                        // "console.debug" and "console.log" are synonyms, that doesn't match the
                        // behavior of browsers in the real world. Chromium only displays "debug"
                        // messages if you enable "Verbose" in the filter dropdown (which is off
                        // by default). As such "console.debug" is the best choice for messages
                        // with a lower severity level than "Information".
                        JsRuntime.InvokeVoid("console.debug", formattedMessage);
                        break;
                    case LogLevel.Information:
                        JsRuntime.InvokeVoid("console.info", formattedMessage);
                        break;
                    case LogLevel.Warning:
                        JsRuntime.InvokeVoid("console.warn", formattedMessage);
                        break;
                    case LogLevel.Error:
                        JsRuntime.InvokeVoid("console.error", formattedMessage);
                        break;
                    case LogLevel.Critical:
                        JsRuntime.InvokeUnmarshalled<string, object>("Blazor._internal.dotNetCriticalError", formattedMessage);
                        break;
                    default:
                        JsRuntime.InvokeVoid("console.log", formattedMessage);
                        break;
                }
            }
        }

        public async Task WriteAsync(LogLevel logLevel, string formattedMessage)
        {
            if (RuntimeInformation.IsOSPlatform(_supportedPlatform))
            {

                switch (logLevel)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                        // Although https://console.spec.whatwg.org/#loglevel-severity claims that
                        // "console.debug" and "console.log" are synonyms, that doesn't match the
                        // behavior of browsers in the real world. Chromium only displays "debug"
                        // messages if you enable "Verbose" in the filter dropdown (which is off
                        // by default). As such "console.debug" is the best choice for messages
                        // with a lower severity level than "Information".
                        await JsRuntime.InvokeVoidAsync("console.debug", formattedMessage);
                        break;
                    case LogLevel.Information:
                        await JsRuntime.InvokeVoidAsync("console.info", formattedMessage);
                        break;
                    case LogLevel.Warning:
                        await JsRuntime.InvokeVoidAsync("console.warn", formattedMessage);
                        break;
                    case LogLevel.Error:
                        await JsRuntime.InvokeVoidAsync("console.error", formattedMessage);
                        break;
                    case LogLevel.Critical:
                        JsRuntime.InvokeUnmarshalled<string, object>("Blazor._internal.dotNetCriticalError", formattedMessage);
                        break;
                    default:
                        await JsRuntime.InvokeVoidAsync("console.log", formattedMessage);
                        break;
                }
            }
        }
    }
}
