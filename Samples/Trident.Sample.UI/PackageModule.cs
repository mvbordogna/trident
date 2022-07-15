using Trident.Contracts.Api.Client;
using Trident.IoC;

namespace Trident.Sample.UI
{
    public class PackageModule : IoCModule
    {
        public override void Configure(IIoCProvider builder)
        {
            builder.RegisterAll<IServiceProxy>(this.TargetAssemblies, LifeSpan.InstancePerLifetimeScope);

        }
    }
}
