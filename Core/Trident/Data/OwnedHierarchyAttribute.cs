using System;

namespace Trident.Data
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class OwnedHierarchyAttribute : Attribute
    {
        public OwnedHierarchyAttribute() { }

        public OwnedHierarchyAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
 
        public string FieldName { get; }
    }  
}
