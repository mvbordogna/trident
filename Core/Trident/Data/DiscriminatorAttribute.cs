using System;

namespace Trident.Data
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DiscriminatorAttribute : Attribute
    {
        public string Value { get; set; }
        public string Property { get; set; }

        public DiscriminatorAttribute(string property, string value)
        {
            Property = property;
            Value = value;
        }
        public DiscriminatorAttribute(string value)
        {
            Value = value;
        }
    }
}
