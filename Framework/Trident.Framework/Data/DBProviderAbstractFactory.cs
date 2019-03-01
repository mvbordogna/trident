using Trident.Data.Contracts;
using System.Data.Common;

namespace Trident.Framework.Data
{
    public class DBProviderAbstractFactory : IDBProviderAbstractFactory
    {
        public DbProviderFactory GetFactory(string provider)
        {
            return DbProviderFactories.GetFactory(provider);
        }
    }
}