using System;
using System.Collections.Generic;
using Trident.IoC;

namespace Trident.EFCore
{
    public class EntityMapFactory : IEntityMapFactory
    {
        private readonly IIoCServiceLocator serviceLocator;

        public EntityMapFactory(IIoCServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public IEnumerable<IEntityMapper> GetMapsFor(Type mapType)
        {
            var genType = typeof(IEntityMapper<>).MakeGenericType(mapType);
            var results = this.serviceLocator.ResolveAllTyped(genType);
            foreach (var m in results)
            {
                yield return m as IEntityMapper;
            }
        }
    }




}