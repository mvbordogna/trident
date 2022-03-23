using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Radzen;
using Trident.Contracts.Api.Client;
using Trident.IoC;

namespace Trident.Samples.Blazor.Client
{
    public class PackageModule : IoCModule
    {
        public override void Configure(IIoCProvider builder)
        {
            RegisterDefaultAssemblyScans(builder);
            builder.RegisterSingleton<DialogService, DialogService>();            
            builder.RegisterSelf();
        }
    }
}
