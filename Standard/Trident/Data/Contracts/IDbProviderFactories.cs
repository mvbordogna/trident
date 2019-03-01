using System.Data.Common;

namespace Trident.Data.Contracts
{
    public interface IDBProviderAbstractFactory
    {
        DbProviderFactory GetFactory(string provider);
    }
}
