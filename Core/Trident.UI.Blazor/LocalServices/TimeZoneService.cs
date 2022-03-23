using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Trident.UI.Blazor.Contracts.Services;

namespace Trident.UI.Blazor.Client
{

    public sealed class TimeZoneService : ITimeZoneService
    {
        private readonly IJSRuntime _jsRuntime;
        private TimeSpan? _userOffset;

        public TimeZoneService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<DateTimeOffset> GetLocalDateTime(DateTimeOffset dateTime)
        {
            if (_userOffset != null)
                return dateTime.ToOffset(_userOffset.Value);

            var offsetInMinutes = await _jsRuntime.InvokeAsync<int>("blazorGetTimezoneOffset");
            _userOffset ??= TimeSpan.FromMinutes(-offsetInMinutes);

            return dateTime.ToOffset(_userOffset.Value);
        }
    }
}
