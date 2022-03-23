using Trident.IoC;

namespace Trident
{
    public class PackageModule : TridentModule
    {
        public override void Configure(IIoCProvider builder)
        {
            base.RegisterDefaultAssemblyScans(builder);
            builder.UsingTridentData();
        }

    }
}
