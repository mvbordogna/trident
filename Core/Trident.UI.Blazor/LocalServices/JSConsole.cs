using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trident.UI.Blazor.Client
{
    public interface IJSConsole
    {
        Task LogAsync(string message);
    }

    public class JSConsole : IJSConsole
    {
        private readonly IJSRuntime JsRuntime;
        public JSConsole(IJSRuntime jSRuntime)
        {
            this.JsRuntime = jSRuntime;
        }

        public async Task LogAsync(string message)
        {
            await this.JsRuntime.InvokeVoidAsync("console.log", message);
        }
    }
}
