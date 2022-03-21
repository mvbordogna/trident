using System.Reflection;
using System.Linq;
using Trident.IoC;

namespace Scholar.Framework.Azure.Common
{
    public static class IIoCAzureExtensions
    {
        public static int IFunctionControllerFactory { get; private set; }
        public static int AzureFunctionControllerFactory { get; private set; }

        public static IIoCProvider RegisterAzureFunctionTypeFactory(this IIoCProvider builder, Assembly[] assemblies)
        {

            var controllerTypes = assemblies.SelectMany(x => x.GetTypes())
                .Where(x => typeof(IFunctionController).IsAssignableFrom(x));

            builder.RegisterBehavior<IFunctionControllerFactory>(() =>
           {
               return new AzureFunctionControllerFactory(controllerTypes);
           }
            , LifeSpan.SingleInstance);
            return builder;
        }
    }
}
