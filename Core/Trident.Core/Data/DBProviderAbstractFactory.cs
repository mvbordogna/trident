using Trident.Data.Contracts;
using System.Data.Common;

namespace Trident.Core.Data
{ 
    public class DBProviderAbstractFactory : IDBProviderAbstractFactory
    {
        public DbProviderFactory GetFactory(string provider)
        {
            return DbProviderFactories.GetFactory(provider);
        }
    }
}