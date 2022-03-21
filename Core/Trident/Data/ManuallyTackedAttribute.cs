using System;

namespace Trident.Data
{ 

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ManuallyTackedAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
