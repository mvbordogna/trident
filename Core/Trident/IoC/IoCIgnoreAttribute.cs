using System;

namespace Trident.IoC
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class IoCIgnoreAttribute : Attribute
    {      
        public IoCIgnoreAttribute() { }
    }   
}
