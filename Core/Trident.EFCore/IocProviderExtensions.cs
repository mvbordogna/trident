using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trident.Common;
using Trident.Contracts;
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
