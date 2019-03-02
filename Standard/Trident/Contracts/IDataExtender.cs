using Trident.Common;
using Trident.IoC;

namespace Trident.Contracts
{
    public interface IDataExtender
    {
        void RegisterSupportedConnections(TridentOptions config, IConnectionStringSettings connStringManager, IIoCProvider provider);
    }
}
