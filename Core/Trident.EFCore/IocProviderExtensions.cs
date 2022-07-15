using System.Reflection;
using Trident.Contracts;
using Trident.Contracts.Configuration;
using Trident.Extensions;
using Trident.IoC;

namespace Trident.EFCore
{
    public static class IocProviderExtensions
    {
        public static void RegisterDataProviderPackages(this IIoCProvider ioc, Assembly[] targetAssemblies, IConnectionStringSettings connStringManager)
        {
            connStringManager.GuardIsNotNull(nameof(connStringManager));
            IDataExtender extender = new DataExtender();
            extender.RegisterSupportedConnections(targetAssemblies, connStringManager, ioc);
        }
    }
}
