using Radzen;
using Trident.Contracts.Api.Client;
using Trident.IoC;

namespace Trident.UI.Blazor
{
    public class PackageModule : IoCModule
    {
        public override void Configure(IIoCProvider builder)
        {
            RegisterDefaultAssemblyScans(builder);
            builder.RegisterSingleton<DialogService, DialogService>();
            builder.RegisterAll<IServiceProxy>(this.TargetAssemblies, LifeSpan.InstancePerLifetimeScope);            
            builder.RegisterSelf();
        }
    }
}
