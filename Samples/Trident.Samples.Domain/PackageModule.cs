using Trident.Configuration;
using Trident.IoC;

namespace Trident.Samples.Domain
{
    public class PackageModule : IoCModule
    {
        public override void Configure(IIoCProvider builder)
        {
            RegisterDefaultAssemblyScans(builder);

            //this may not be working completely as expected, since registerall only registers directly implemented interfaces currently
            builder.RegisterAll<ICoreConfiguration>(TargetAssemblies);
            builder.RegisterAll<IFactory>(TargetAssemblies);
            //builder.RegisterAll<IGenericManager>(TargetAssemblies);
        }
    }
}
