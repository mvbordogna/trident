using Trident.Contracts.Api.Client;
using Trident.IoC;

namespace Trident.UI.Client
{
    public class PackageModule : IoCModule
    {
        public override void Configure(IIoCProvider builder)
        {
            builder.RegisterAll<IServiceProxy>(this.TargetAssemblies, LifeSpan.InstancePerLifetimeScope);

        }
    }
}
