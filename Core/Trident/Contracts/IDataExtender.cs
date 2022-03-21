using System.Reflection;
using Trident.Common;
using Trident.IoC;

namespace Trident.Contracts
{
    public interface IDataExtender
    {
        void RegisterSupportedConnections(Assembly[] targetAssemblies, IConnectionStringSettings connStringManager, IIoCProvider provider);
    }
}
