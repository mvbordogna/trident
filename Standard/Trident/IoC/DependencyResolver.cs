using System;
using System.Collections.Generic;
using System.Text;

namespace Trident.IoC
{
    public static class Dependency
    {
        internal static void SetServiceLocator(IIoCServiceLocator locator)
        {
            Resolver = locator;
        }
        
        public static IIoCServiceLocator Resolver { get; private set; }
    }
}
