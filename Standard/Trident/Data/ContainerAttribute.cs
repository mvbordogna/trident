using System;

namespace Trident.Data
{

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ContainerAttribute : Attribute
    {
        // This is a positional argument
        public ContainerAttribute(string name)
        {
            this.Name = name;
        }

        // This is a named argument
        public string Name { get; set; }
    }


}
