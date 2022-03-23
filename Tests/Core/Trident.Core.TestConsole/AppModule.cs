using Autofac;
using Trident.IoC;

namespace Trident.Core.TestConsole
{
    class AppModule : IoCModule
    {
        public override void Configure(IIoCProvider builder)
        {
            base.Configure(builder);
        }
    }
}
