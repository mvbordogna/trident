using Trident.IoC;

namespace Trident
{
    public class PackageModule : ModuleBase
    {
        public override void Configure(IIoCProvider builder)
        {
            base.RegisterDefaultAssemblyScans(builder);
            builder.UsingTridentData();
        }

    }
}
