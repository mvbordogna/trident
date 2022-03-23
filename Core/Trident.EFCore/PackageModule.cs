using Trident.Data.Contracts;
using Trident.IoC;

namespace Trident.EFCore
{
    public class PackageModule : IoCModule
    {
      
        public override void Configure(IIoCProvider builder)
        {
            builder.UsingTridentData();            
            builder.Register<EFQueryableHelper, IQueryableHelper>(LifeSpan.SingleInstance);
        }
    }
}
