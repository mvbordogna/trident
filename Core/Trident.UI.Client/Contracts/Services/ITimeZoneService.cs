using System;
using System.Threading.Tasks;
using Trident.Contracts.Api.Client;

namespace Trident.UI.Blazor.Contracts.Services
{
    public interface ITimeZoneService : IServiceProxy
    {
        Task<DateTimeOffset> GetLocalDateTime(DateTimeOffset dateTime);
    }
}
